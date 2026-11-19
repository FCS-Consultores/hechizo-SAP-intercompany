## Context

Ver `proposal.md` para el problema. Estado actual de la réplica de actualización (`MCH_IC/Data/Intercompany.cs`):

- `ActualizarEnOrdCompra` (activa, `Program.cs` ≈L602-612) y `ActualizarEnOrdCompraV2` (solo TEST) arman el mismo payload: cabecera + `DocumentLines` con `ItemCode, Quantity, TaxCode, LineTotal, LineNum, Price AS UnitPrice` desde `RDR1`.
- Se envía con `PATCH PurchaseOrders(n)` y el header `B1S-ReplaceCollectionsOnPatch: true` (`EnviaSapPorBasicAuth`, parámetro `Usar = "Y"`; con `"N"` no se agrega el header).
- La V2 ya llama a `ComparePricesAndLogActualizacion`, que lee `POR1` (`LineNum, ItemCode, Price`) del destino y solo registra diferencias en el log.
- Evidencia: el 0 ocurre únicamente en líneas existentes cuyo `ItemCode` cambió, sin importar el artículo; todos los artículos tienen precio en su lista; borrar y recrear la línea funciona. Hipótesis de trabajo: SAP reinicia la línea al cambiar `ItemCode` sobre un `LineNum` existente y descarta el `UnitPrice` del payload.

## Goals / Non-Goals

**Goals:**
- Que ninguna OC destino quede con precio distinto al origen al terminar el ciclo de actualización.
- Corrección inmediata (mismo ciclo, mismo documento), porque el usuario no puede arrastrar una OC con precio 0 a la factura.
- Error explícito, no silencioso, si no se logra corregir.

**Non-Goals:**
- No cambiar el flujo de creación (`ProcesarEnOrdCompra`) ni facturas de proveedor.
- No reescribir el modelo de datos ni agregar campos UDF nuevos.
- No rediseñar la comparación para otros campos (cantidad, impuesto).

## Decisions

**1. Segundo `PATCH` solo de precio (en vez de borrar y recrear la línea).**
Borrar/recrear cambia el `LineNum` y rompe la correspondencia con `RDR1` del origen, además de ser frágil con documentos base. El segundo `PATCH` no toca la estructura de las líneas. Alternativa descartada: reordenar `UnitPrice` antes de `ItemCode` en el JSON; es barato pero improbable que funcione si el reinicio lo hace SAP y no el orden de deserialización (se puede probar durante la validación manual sin costo).

**2. El segundo `PATCH` va sin `B1S-ReplaceCollectionsOnPatch` (`Usar = "N"`).**
Con el header, SAP volvería a procesar la colección completa y repetiría el problema. Sin él, la colección se actualiza por posición en el arreglo, por lo que el payload debe incluir un objeto `{}` por cada línea que no se corrige y `{ "UnitPrice": x }` en las que sí. **A validar en la prueba manual contra una sociedad de test antes de implementar** (si el emparejamiento resulta por `LineNum` y no por posición, el payload se simplifica a `{ LineNum, UnitPrice }`).

**3. Verificar leyendo `POR1` del destino, no la respuesta del `PATCH`.**
Se reutiliza la consulta de `ComparePricesAndLogActualizacion` (`LineNum, ItemCode, Price`), que ya existe. Se refactoriza para devolver la lista de diferencias (hoy solo escribe al log) y se usa tanto antes como después de la corrección. Alternativa descartada: confiar en la respuesta del `PATCH`; con `204 No Content` no trae datos de líneas.

**4. Tolerancia de comparación a 6 decimales**, igual que el código actual (`PRICE_DECIMALS = 6`), para evitar falsos positivos por redondeo.

**5. Flujo (todo dentro del mismo ciclo del documento):**

```
PATCH 1 (como hoy)
   └─ ok → leer POR1 destino
            ├─ sin diferencias → éxito (flujo actual)
            └─ con diferencias → PATCH 2 (solo UnitPrice de esas líneas, Usar="N")
                                  └─ leer POR1 de nuevo
                                       ├─ sin diferencias → éxito (log: líneas corregidas)
                                       └─ con diferencias → U_ComentarioError = error
                                                            de precio (no "éxito"); log detallado
```

**6. Consolidar V1 y V2 en una sola función.** Hoy son casi idénticas (V2 solo agrega la comparación); mantener ambas duplicaría la corrección. Se conserva el nombre `ActualizarEnOrdCompra` para no tocar las llamadas de `Program.cs` y se elimina la V2 y su línea comentada.

## Risks / Trade-offs

- [El segundo `PATCH` no emparejando por posición como se supone] → Validación manual previa (ver tareas); si falla, caer a borrar y recrear la línea con manejo explícito de `LineNum`.
- [Ventana de inconsistencia entre PATCH 1 y PATCH 2: la OC existe unos instantes con precio 0] → Aceptable: la corrección ocurre en el mismo ciclo, en segundos; el caso se mitiga corriendo la verificación inmediatamente tras el PATCH 1.
- [Un usuario edita la OC destino entre ambos PATCH] → Poco probable en esa ventana; si pasara, la verificación final detecta la diferencia y se reporta como error.
- [Segundo `PATCH` falla (documento bloqueado, período cerrado)] → Se reporta en `U_ComentarioError` con el mensaje de SAP; la OV queda con `U_Actualiza = 'N'` como hoy, por lo que el aviso es el mecanismo de reintento manual.
- [Más llamadas a Service Layer] → Solo cuando hay diferencia; el caso normal añade una lectura a `POR1`.
