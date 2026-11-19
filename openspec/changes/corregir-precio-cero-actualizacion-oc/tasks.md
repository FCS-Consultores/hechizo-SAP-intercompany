## 1. Validación manual previa (bloquea el resto)

- [ ] 1.1 En una sociedad de test, reproducir el bug con `PATCH PurchaseOrders(n)` cambiando `ItemCode` de una línea existente (con `B1S-ReplaceCollectionsOnPatch: true`) y verificar en `POR1` que `Price = 0`
- [ ] 1.2 Probar un segundo `PATCH` sin el header con `DocumentLines: [{}, {UnitPrice: x}]` y verificar en `POR1` que solo la línea indicada queda con el precio correcto y las demás no cambian; documentar si el emparejamiento es por posición o por `LineNum` y ajustar `design.md` (Decisión 2) con el resultado
- [ ] 1.3 (Opcional, barato) Probar si poner `UnitPrice` antes de `ItemCode` en el JSON evita el 0; registrar el resultado en `design.md`

## 2. Verificación de precios del destino

- [ ] 2.1 Refactorizar `ComparePricesAndLogActualizacion` en `MCH_IC/Data/Intercompany.cs` para que devuelva la lista de diferencias (`LineNum`, `ItemCode`, precio esperado, precio obtenido) además de registrarla en el log; verificar compilando y que el log conserva el formato actual `DIFERENCIA DE PRECIOS - ACTUALIZACION`
- [ ] 2.2 Mantener la tolerancia de 6 decimales (`PRICE_DECIMALS`) y verificar que precios iguales con distinto redondeo no generan diferencia

## 3. Corrección inmediata

- [ ] 3.1 Agregar un método que arme el payload del segundo `PATCH` (solo `UnitPrice` de las líneas con diferencia, según el emparejamiento validado en 1.2) y verificar con un caso de 3 líneas donde solo la línea 2 difiere
- [ ] 3.2 En `ActualizarEnOrdCompra`, tras un `PATCH` exitoso, leer `POR1`, y si hay diferencias ejecutar el segundo `PATCH` con `EnviaSapPorBasicAuth(..., Usar: "N")`; verificar en el caso de las gorras (artículo A→B, mismo precio) que `POR1.Price` queda igual al origen en el mismo ciclo
- [ ] 3.3 Tras el segundo `PATCH`, volver a leer `POR1` y confirmar que no hay diferencias; verificar que en una actualización sin cambio de artículo no se ejecuta ningún segundo `PATCH`

## 4. Reporte de errores

- [ ] 4.1 Si tras la corrección persisten diferencias (o el segundo `PATCH` falla), escribir en `ORDR.U_ComentarioError` del origen un mensaje de error de precio con línea, artículo, precio esperado y obtenido, sin usar el texto "OrdComp Actualizada con éxito"; verificar forzando un fallo (ej. documento destino bloqueado) y revisando el campo y el log
- [ ] 4.2 Mantener el mensaje de éxito y el registro en `LogJson` sin cambios cuando no hay diferencias; verificar que el caso normal se comporta como antes

## 5. Consolidación

- [ ] 5.1 Eliminar `ActualizarEnOrdCompraV2` y la línea comentada de `MCH_IC/Program.cs` (≈L614) para dejar una sola implementación; verificar que la solución compila y las 11 llamadas por sociedad siguen intactas
- [ ] 5.2 Prueba de integración en una sociedad de test: cambio de artículo en una línea, cambio de cantidad sin cambio de artículo, y OV con varias líneas modificadas a la vez; verificar `POR1` y `U_ComentarioError` en cada caso, y que la OC resultante se puede copiar a factura de proveedor sin precio 0
