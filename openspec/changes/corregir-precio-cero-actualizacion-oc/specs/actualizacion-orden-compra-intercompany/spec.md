## Purpose

Garantiza que la réplica de actualizaciones de una Orden de Venta de la casa matriz hacia la Orden de Compra de cada sociedad hija deje los precios por línea idénticos al origen, para que el documento sea utilizable de inmediato por los usuarios de SAP (incluida su copia a factura de proveedor).

## ADDED Requirements

### Requirement: Precio por línea idéntico al origen tras actualizar
Tras replicar una actualización de Orden de Venta hacia su Orden de Compra, el sistema SHALL dejar el precio unitario de cada línea de la OC igual al precio de la línea correspondiente (mismo `LineNum`) de la OV de origen, incluidas las líneas cuyo artículo fue reemplazado por otro.

#### Scenario: Línea con artículo reemplazado conserva el precio
- **WHEN** en la OV de origen se cambia el artículo de una línea existente por otro con el mismo precio y se replica la actualización
- **THEN** la línea correspondiente de la OC destino tiene el nuevo artículo y el mismo precio que la OV de origen, no 0

#### Scenario: Línea sin cambio de artículo no se altera
- **WHEN** se replica una actualización en la que el artículo de una línea no cambió
- **THEN** el precio de esa línea en la OC destino coincide con el origen y no se realizan modificaciones adicionales sobre ella

### Requirement: Validación y corrección inmediatas
El sistema SHALL verificar los precios de la OC destino y corregir cualquier diferencia dentro del mismo ciclo de procesamiento de la actualización, antes de dar el documento por actualizado y sin depender de un proceso posterior.

#### Scenario: Diferencia detectada y corregida en el mismo ciclo
- **WHEN** tras aplicar la actualización una línea de la OC destino tiene un precio distinto al de origen
- **THEN** el sistema corrige el precio de esa línea en el mismo ciclo y vuelve a verificar que coincide

#### Scenario: Sin diferencias no se corrige nada
- **WHEN** todos los precios de la OC destino coinciden con el origen tras la actualización
- **THEN** el sistema no realiza ninguna modificación adicional sobre la OC

### Requirement: Corrección limitada a precios
La corrección SHALL modificar únicamente el precio unitario de las líneas con diferencia y NO SHALL alterar el resto de las líneas ni los demás campos del documento destino.

#### Scenario: Otras líneas permanecen intactas
- **WHEN** se corrige el precio de una línea de una OC con varias líneas
- **THEN** las demás líneas conservan artículo, cantidad, precio e impuesto tal como quedaron tras la actualización

### Requirement: Error visible si el precio no se corrige
Si tras la corrección persiste alguna diferencia de precio, el sistema NO SHALL reportar la actualización como exitosa; SHALL registrar el error en el campo de comentario de error de la OV de origen y en el log, indicando línea, artículo, precio esperado y precio obtenido.

#### Scenario: Corrección fallida
- **WHEN** después del intento de corrección una línea de la OC destino sigue con precio distinto al de origen
- **THEN** el comentario de error de la OV de origen indica la falla de precio, no dice "actualizada con éxito" y el log detalla la línea afectada

#### Scenario: Actualización exitosa sin diferencias
- **WHEN** la actualización y la verificación terminan sin diferencias de precio
- **THEN** la OV de origen queda marcada como actualizada con el mensaje de éxito existente
