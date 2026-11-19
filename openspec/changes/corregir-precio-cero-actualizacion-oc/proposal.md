## Why

Cuando una Orden de Venta de la casa matriz se actualiza manualmente en SAP y se cambia el artículo de una línea existente (ej. misma gorra, otro color/código, mismo precio), la réplica hacia la Orden de Compra de la sociedad hija deja esa línea con `Price = 0` en `POR1`, aunque el JSON enviado por Service Layer lleva el precio correcto (confirmado en el log de JSON). SAP reinicia la línea al cambiar el `ItemCode` sobre un `LineNum` existente y descarta el `UnitPrice` del payload. Las líneas nuevas o sin cambio de artículo no se ven afectadas.

El error es silencioso (`U_ComentarioError` dice "OrdComp Actualizada con éxito") y el usuario de SAP no puede arrastrar una OC con precio 0 hacia la factura de proveedor, por lo que la detección y corrección deben ocurrir en el mismo ciclo de actualización, no después.

## What Changes

- Después de cada `PATCH PurchaseOrders(n)` exitoso en la réplica de actualización, leer `POR1` del destino y comparar `Price` por `LineNum` contra el precio enviado desde `RDR1`.
- Para las líneas con diferencia, ejecutar de inmediato un segundo `PATCH` (sin `B1S-ReplaceCollectionsOnPatch`) que fije solo el `UnitPrice` de esas líneas.
- Volver a verificar `POR1` tras el segundo `PATCH`; si persiste la diferencia, no marcar el documento como actualizado con éxito: dejar el error visible en `U_ComentarioError` de la OV de origen y en el log.
- Consolidar `ActualizarEnOrdCompra` (activa) y `ActualizarEnOrdCompraV2` (solo TEST, con comparación) en una única implementación con la validación y corrección incluidas.

## Capabilities

### New Capabilities
- `actualizacion-orden-compra-intercompany`: réplica de actualizaciones de OV (origen) hacia OC (sociedades hijas), incluyendo la garantía de que los precios por línea del destino coinciden con el origen.

### Modified Capabilities
<!-- Ninguna: no hay specs previas en openspec/specs/ -->

## Impact

- Código: `MCH_IC/Data/Intercompany.cs` — `ActualizarEnOrdCompra` (≈L1675), `ActualizarEnOrdCompraV2` (≈L1853), `ComparePricesAndLogActualizacion` (≈L5887), `EnviaSapPorBasicAuth` (≈L4661); `MCH_IC/Program.cs` (≈L602-614) para las llamadas por sociedad.
- Sistemas: Service Layer de cada sociedad destino (un `PATCH` adicional solo cuando hay diferencia); tabla `POR1` (lectura); campo `U_ComentarioError` de `ORDR` en origen.
- Sin cambios de esquema ni de configuración.
- Fuera de alcance: facturas de proveedor (`ActualizarEnFacturaProvee` no envía líneas) y el flujo de creación (`ProcesarEnOrdCompra`), donde el precio sí llega bien.
