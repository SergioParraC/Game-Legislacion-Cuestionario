# ?? Trivia Challenge - Juego de Preguntas y Respuestas

Juego de trivia desarrollado en C# con Windows Forms y SQLite.

## ?? Descripción

Trivia Challenge es un juego de preguntas de cultura general con dificultad progresiva, power-ups estratégicos y penalizaciones dinámicas.

## ?? Características Principales

### Sistema de Juego
- **4 Niveles de dificultad** con complejidad incremental
- **Sistema de puntuación**: Base de 100 puntos + bonificaciones (hasta 500 por pregunta)
- **Temporizador dinámico**: De 15 a 7 segundos según el nivel
- **Rachas de aciertos**: Bonificaciones por respuestas consecutivas correctas
- **Jefe Final**: Desafío especial con penalizaciones constantes

### Power-Ups (Limitados)
| Power-Up | Cantidad Inicial | Efecto |
|----------|------------------|---------|
| ?? Eliminar Opción | 3 | Quita una respuesta incorrecta |
| ? +5 Segundos | 3 | Añade 5 segundos al temporizador |
| ?? Reintento | 2 | Permite volver a responder |
| ? Doble Puntos | 2 | Duplica los puntos de la pregunta |

### Sistema de Penalizaciones
El juego actúa como rival aplicando penalizaciones:
- **Menos Tiempo**: Reduce 3 segundos del temporizador
- **Orden Confuso**: Mezcla las respuestas
- **Bloqueo**: No se pueden usar power-ups
- **Aplicación**: Al fallar, aleatoriamente, o en niveles altos

### Niveles

| Nivel | Tiempo | Preguntas | Penalizaciones |
|-------|--------|-----------|----------------|
| 1 | 15s | 5 | Ninguna |
| 2 | 12s | 7 | Leves (20%) |
| 3 | 10s | 10 | Frecuentes (40%) |
| Jefe Final | 7s | 3 | Constantes |

## ??? Arquitectura del Proyecto

```
Solution/
??? Foundation/          # Capa de Datos
?   ??? DatabaseConnection.cs
?   ??? Models/
?   ?   ??? Question.cs
?   ?   ??? GameProgressModel.cs
?   ??? Repositories/
?       ??? QuestionRepository.cs
?       ??? GameProgressRepository.cs
?
??? Facade/             # Capa de Comunicación
?   ??? GameFacade.cs
?
??? Game/               # Lógica y UI
    ??? Enums/
    ?   ??? PowerUpType.cs
    ?   ??? PenaltyType.cs
    ??? Models/
    ?   ??? Player.cs
    ?   ??? Level.cs
    ?   ??? PowerUp.cs
    ?   ??? Penalty.cs
    ??? Managers/
    ?   ??? GameManager.cs
    ?   ??? ScoreManager.cs
    ??? Forms/
        ??? MainMenuForm.cs
        ??? GameForm.cs
        ??? LeaderboardForm.cs
```

## ?? Tecnologías

- **Lenguaje**: C# 14.0
- **Framework**: .NET 10
- **UI**: Windows Forms
- **Base de Datos**: SQLite
- **Paquetes NuGet**: 
  - Microsoft.Data.Sqlite (10.0.3)

## ?? Sistema de Puntuación

```
Puntos = Puntos Base (100)
       + (Tiempo Restante × 2)
       + (Racha × 50)
       + (Doble Puntos × 2)
       
Penalización por error: -25 puntos
Puntuación máxima por pregunta: 500 puntos
```

## ?? Modelo de Datos

### Tabla Questions
- Id (INTEGER PRIMARY KEY)
- Text (TEXT)
- CorrectAnswerIndex (INTEGER)
- Options (TEXT) - Separadas por pipe (|)
- Difficulty (INTEGER) - 1-4
- IsBossQuestion (INTEGER) - 0 o 1

### Tabla GameProgress
- Id (INTEGER PRIMARY KEY)
- PlayerName (TEXT)
- CurrentLevel (INTEGER)
- MaxScore (INTEGER)
- DatePlayed (TEXT)

## ?? Cómo Ejecutar

1. **Clonar el repositorio**
2. **Abrir la solución** en Visual Studio 2022 o superior
3. **Compilar** la solución (Ctrl + Shift + B)
4. **Ejecutar** el proyecto Game (F5)

## ?? Funcionalidades Implementadas

? Sistema completo de preguntas con SQLite
? 4 niveles con dificultad progresiva
? Power-ups funcionales y limitados
? Sistema de penalizaciones automáticas
? Temporizador dinámico con cambio de color
? Jefe final con mecánicas especiales
? Guardado automático de progreso
? Tabla de líderes (Top 10)
? Interfaz gráfica profesional
? Sistema de rachas y bonificaciones
? Validación de respuestas correctas/incorrectas

## ?? Interfaz de Usuario

- **Menú Principal**: Inicio de partida, tabla de líderes y salir
- **Pantalla de Juego**: Pregunta, opciones, temporizador, puntuación, racha y power-ups
- **Tabla de Líderes**: Top 10 mejores puntuaciones con datos del jugador

## ?? Dependencias entre Proyectos

```
Game ? Facade ? Foundation
Game ? Foundation (directo)
```

## ?? Reglas del Juego

1. Responde correctamente para acumular puntos
2. Usa power-ups estratégicamente (son limitados)
3. El sistema aplica penalizaciones al fallar o aleatoriamente
4. Completa los 4 niveles para ganar
5. No todos los power-ups se pueden combinar
6. El tiempo es crucial: más rápido = más puntos

## ????? Desarrollo

El proyecto sigue una arquitectura en capas:
- **Foundation**: Acceso a datos y repositorios
- **Facade**: Abstracción de la lógica de datos
- **Game**: Lógica de negocio e interfaz de usuario

## ?? Licencia

Proyecto educativo - Universidad

## ?? Autor

Desarrollado como proyecto académico
