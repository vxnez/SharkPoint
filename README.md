# Punto de Venta - Proyecto de Tópicos Avanzados de Programación

Este proyecto es una aplicación de punto de venta desarrollada en .NET MAUI como parte de la materia **Tópicos Avanzados de Programación**. Permite gestionar inventario, registrar ventas y consultar el historial diario de productos vendidos.

## Características principales

- **Inventario de productos:**  
  Registra, edita y elimina productos en una base de datos local.  
  Permite importar y exportar productos desde/hacia archivos Excel.

- **Registro de ventas:**  
  Busca productos por ID y agrega varios productos a una venta.  
  Calcula y muestra el subtotal y total de la compra.

- **Historial diario:**  
  Selector de fecha para consultar productos vendidos en un día específico.  
  Muestra el total de ganancias diarias.

## Tecnologías utilizadas

- [.NET 9](https://dotnet.microsoft.com/)
- [.NET MAUI](https://learn.microsoft.com/dotnet/maui/)
- [SQLite](https://www.sqlite.org/index.html) para almacenamiento local
- [NPOI](https://github.com/tonyqus/npoi) para manejo de archivos Excel

## Instalación

1. Clona este repositorio:
   git clone https://github.com/vxnez/SharkPoint.git
2. Abre la solución en Visual Studio.
3. Restaura los paquetes NuGet.
4. Compila y ejecuta el proyecto en el emulador o dispositivo de tu preferencia.

## Uso

- **Inventario:**  
  Agrega, edita o elimina productos. Usa la opción de importar/exportar para manejar archivos Excel.

- **Registro de ventas:**  
  Busca productos por ID, agrégalos a la venta y finaliza la compra para ver el total.

- **Historial diario:**  
  Selecciona una fecha para ver los productos vendidos y el total de ganancias de ese día.


## Licencia

Este proyecto es solo para fines educativos.
