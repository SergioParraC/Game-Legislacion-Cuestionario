# ?? Instrucciones de Ejecución - Trivia Challenge

## ? Requisitos Previos

- Visual Studio 2022 o superior
- .NET 10 SDK instalado
- Windows 10/11

## ?? Pasos para Ejecutar el Juego

### 1. Verificar que todo está correctamente compilado

Ya hemos realizado la compilación y todo está funcionando correctamente.

### 2. Ejecutar el Juego

**Opción A: Desde Visual Studio**
1. Abre la solución en Visual Studio
2. Establece el proyecto **Game** como proyecto de inicio (clic derecho ? "Establecer como proyecto de inicio")
3. Presiona `F5` o haz clic en el botón "Iniciar"

**Opción B: Desde la terminal**
```bash
dotnet run --project Game/Game.csproj
```

### 3. Jugar

1. **Menú Principal**
   - Ingresa tu nombre (por defecto: "Jugador 1")
   - Haz clic en "? Jugar" para comenzar
   - O consulta la "?? Tabla de Líderes"

2. **Durante el Juego**
   - Lee la pregunta
   - Selecciona la respuesta correcta
   - Usa power-ups estratégicamente (botones en la parte inferior)
   - Observa el temporizador (cambia a rojo cuando quedan ?5s)
   - Completa cada nivel para avanzar

3. **Power-Ups Disponibles**
   - ?? **Eliminar Opción** (3): Quita una respuesta incorrecta
   - ? **+5 Segundos** (3): Añade tiempo al temporizador
   - ?? **Reintento** (2): Permite responder de nuevo
   - ? **Doble Puntos** (2): Duplica los puntos de la pregunta

4. **Información en Pantalla**
   - **Nivel**: Nivel actual (1-4)
   - **Puntos**: Puntuación total acumulada
   - **Racha**: Respuestas correctas consecutivas
   - **Temporizador**: Tiempo restante para responder
   - **Penalización**: Mensaje de advertencia si hay penalizaciones activas

## ?? Consejos de Juego

1. **Gestiona tus power-ups**: Son limitados, úsalos sabiamente
2. **Responde rápido**: Más tiempo restante = más puntos
3. **Mantén la racha**: Las respuestas consecutivas correctas dan bonificación
4. **Cuidado con las penalizaciones**: Aparecen al fallar o aleatoriamente en niveles altos
5. **Prepárate para el Jefe Final**: Nivel 4 tiene penalizaciones constantes

## ?? Sistema de Puntuación

```
Puntos Base: 100
+ Tiempo Restante × 2
+ Racha × 50
+ Doble Puntos (×2 si está activo)
-----------------------------------
Máximo por pregunta: 500 puntos
Penalización por error: -25 puntos
```

## ??? Archivos Generados

Al ejecutar el juego por primera vez, se creará:
- **GameDatabase.db**: Base de datos SQLite en la carpeta de ejecución
  - Contiene 18 preguntas precargadas
  - Guarda tu progreso y puntuaciones

## ?? Solución de Problemas

### El juego no inicia
- Verifica que el proyecto Game esté establecido como proyecto de inicio
- Compila toda la solución: `Ctrl + Shift + B`

### Error de base de datos
- La base de datos se crea automáticamente la primera vez
- Si hay problemas, elimina `GameDatabase.db` y vuelve a ejecutar

### No aparecen preguntas
- La base de datos se inicializa con 18 preguntas de ejemplo
- Verifica que el archivo `GameDatabase.db` se haya creado

## ?? Estructura de Archivos en Ejecución

```
Game/bin/Debug/net10.0-windows/
??? Game.exe
??? GameDatabase.db          ? Se crea automáticamente
??? Foundation.dll
??? Facade.dll
??? [Otros archivos del framework]
```

## ?? Objetivo del Juego

**Completar los 4 niveles con la mayor puntuación posible**

- Nivel 1: 5 preguntas (15s cada una)
- Nivel 2: 7 preguntas (12s cada una)
- Nivel 3: 10 preguntas (10s cada una)
- Nivel 4 (Jefe Final): 3 preguntas (7s cada una)

## ?? Tabla de Líderes

- Accede desde el menú principal
- Muestra los Top 10 mejores puntajes
- Incluye: Posición, Nombre, Nivel alcanzado, Puntos y Fecha

## ?? Soporte

Si encuentras algún problema:
1. Verifica que todos los proyectos tengan las referencias correctas
2. Asegúrate de tener .NET 10 instalado
3. Revisa la consola de Visual Studio para mensajes de error

---

**¡Disfruta del juego! ??**
