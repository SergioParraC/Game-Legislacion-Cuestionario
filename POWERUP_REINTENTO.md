# ?? Documentación del Power-Up: CAMBIO DE PREGUNTA

## ?? Descripción

El power-up **Cambio de Pregunta** (anteriormente llamado "Reintento") permite al jugador cambiar la pregunta actual por una pregunta de reserva si falla en su respuesta.

## ?? Funcionamiento Completo

### **Concepto: Pregunta de Reserva**

Cada nivel (excepto el Jefe Final) tiene **una pregunta adicional de reserva**:

| Nivel | Preguntas Activas | Pregunta de Reserva | Total Cargadas |
|-------|-------------------|---------------------|----------------|
| 1 | 5 | 1 | 6 |
| 2 | 7 | 1 | 8 |
| 3 | 10 | 1 | 11 |
| Jefe Final | 3 | 0 (No disponible) | 3 |

### **1. Activación del Power-Up**

Cuando el jugador hace clic en el botón **?? Cambio de Pregunta**:

```csharp
// GameManager.cs
case PowerUpType.Retry:
    IsRetryAvailable = true;  // Se activa la protección
    break;
```

**Mensaje mostrado:**
> ¡CAMBIO DE PREGUNTA activado!
> 
> Si fallas, se cargará una pregunta diferente sin penalización.

- ? Se consume 1 power-up del inventario del jugador
- ? Se activa la bandera `IsRetryAvailable = true`
- ? El jugador ahora tiene protección para cambiar de pregunta

### **2. Respuesta CORRECTA con Cambio de Pregunta Activo**

Si el jugador responde **correctamente** con el power-up activo:

```csharp
// GameManager.cs CheckAnswer()
if (IsRetryAvailable)
{
    IsRetryAvailable = false;  // Se desactiva sin efecto
}
```

**Comportamiento:**
- ? Gana puntos normalmente
- ? Mantiene su racha
- ? **Pierde el power-up** (se consumió sin usar)
- ? Avanza a la siguiente pregunta

?? **Importante:** Si respondes bien, el power-up se pierde sin efecto.

### **3. Respuesta INCORRECTA con Cambio de Pregunta Activo**

Si el jugador responde **incorrectamente** con el power-up activo:

```csharp
// GameForm.cs
if (_gameManager.IsRetryAvailable)
{
    MessageBox.Show("? Incorrecto. ¡CAMBIO DE PREGUNTA activado!\n\nSe cargará una nueva pregunta.");
    _gameManager.ChangeToReserveQuestion();
}
```

**Comportamiento:**
- ? **NO** se pierden puntos (-25)
- ? **NO** se reinicia la racha
- ? **NO** se aplican penalizaciones aleatorias
- ? **NO** se avanza a la siguiente pregunta
- ? **SÍ** se cambia a la pregunta de reserva
- ? **SÍ** se reinicia el temporizador
- ? **SÍ** se desactiva el power-up (ya usado)

### **4. Cambio a Pregunta de Reserva**

El método `ChangeToReserveQuestion()` realiza el cambio:

```csharp
// GameManager.cs
public void ChangeToReserveQuestion()
{
    if (CurrentLevel.ReserveQuestion != null)
    {
        CurrentQuestion = CurrentLevel.ReserveQuestion;
        TimeRemaining = CurrentLevel.TimePerQuestion;
        CurrentLevel.ReserveQuestion = null;  // Solo se usa una vez
        IsRetryAvailable = false;
        ShuffleOptions();
    }
}
```

---

## ?? **Flujo Completo del Cambio de Pregunta**

```
1. Usuario activa "?? Cambio de Pregunta"
         ?
2. Se consume 1 power-up del inventario
         ?
3. IsRetryAvailable = true
         ?
4. Usuario responde una pregunta
         ?
   ¿Respuesta correcta?
         ?
   ?????????????
   SÍ         NO
   ?          ?
Power-up   Cambiar a
perdido    pregunta de
sin usar   reserva
   ?          ?
Avanzar    Nueva pregunta
siguiente  mismo puntaje
pregunta   misma racha
```

---

## ?? **Características Importantes**

### **Protección Total si Fallas**
Cuando está activo y fallas:
- ? No pierdes puntos (-25)
- ? No pierdes racha
- ? No recibes penalizaciones aleatorias
- ? Obtienes una pregunta diferente

### **Pérdida sin Efecto si Aciertas**
- ?? Si respondes **correctamente** con el power-up activo, lo **pierdes sin usarlo**
- ?? Es importante activarlo **solo si no estás seguro**

### **Uso Único por Nivel**
- ?? Solo hay **1 pregunta de reserva por nivel**
- ?? Una vez usada, no se puede volver a cambiar de pregunta
- ?? No disponible en el Jefe Final (Nivel 4)

### **Estrategia Recomendada**
1. ? **Activar SOLO si tienes dudas** (riesgo de perderlo si aciertas)
2. ? Usar en preguntas difíciles donde puedas fallar
3. ? Guardar para Nivel 3 (10 preguntas difíciles)
4. ? **NO** activar si estás seguro de la respuesta
5. ? **NO** disponible en Jefe Final

---

## ?? **Ejemplos de Uso**

### **Caso 1: Uso Exitoso del Power-Up**
```
1. Nivel 2, Pregunta 5: "¿En qué año...?"
2. Usuario tiene dudas entre 2 opciones
3. Activa "?? Cambio de Pregunta"
4. Responde incorrectamente
5. ¡NO pierde puntos ni racha! ?
6. Se carga pregunta de reserva: "¿Cuál es el océano...?"
7. Responde correctamente
8. ¡Gana puntos manteniendo la racha! ??
```

### **Caso 2: Power-Up Perdido sin Usar**
```
1. Nivel 1, Pregunta 3: "¿Cuál es la capital de España?"
2. Usuario activa "?? Cambio de Pregunta" (por precaución)
3. Responde: "Madrid" ? (Correcto)
4. Gana puntos normalmente
5. ? Pero perdió el power-up sin usarlo
```

### **Caso 3: No Disponible en Jefe Final**
```
1. Nivel 4 (Jefe Final), Pregunta 1
2. Usuario intenta activar "?? Cambio de Pregunta"
3. Mensaje: "No hay pregunta de reserva disponible en este nivel"
4. Power-up NO se consume
```

---

## ?? **Implementación Técnica**

### **Archivos Modificados:**

1. **Level.cs** - Agregada propiedad `ReserveQuestion`
2. **GameFacade.cs** - Carga N+1 preguntas por nivel
3. **GameManager.cs** - Métodos `StartLevel()`, `CheckAnswer()`, `ChangeToReserveQuestion()`
4. **GameForm.cs** - Lógica de cambio de pregunta en `OptionButton_Click`
5. **DatabaseConnection.cs** - Preguntas adicionales en la base de datos

### **Variables de Estado:**

```csharp
public bool IsRetryAvailable { get; set; }  // Power-up activo
public Question ReserveQuestion { get; set; }  // Pregunta de reserva del nivel
```

---

## ?? **Comparación: Con vs Sin Cambio de Pregunta**

| Aspecto | Sin Power-Up | Con Power-Up (Respuesta Incorrecta) | Con Power-Up (Respuesta Correcta) |
|---------|--------------|-------------------------------------|-----------------------------------|
| Puntos perdidos | -25 | 0 | +100-500 |
| Racha | Se reinicia | Se mantiene | Aumenta |
| Penalizaciones | Se aplican | NO se aplican | N/A |
| Pregunta | Avanza | Cambia a reserva | Avanza |
| Power-up | N/A | Se usa | Se pierde |

---

## ?? **Limitaciones**

1. ? Cantidad inicial: **2 usos** por partida
2. ? Solo **1 pregunta de reserva** por nivel
3. ? **NO disponible** en Jefe Final (Nivel 4)
4. ? Se debe activar **ANTES** de responder
5. ? Se pierde si respondes **correctamente**
6. ? No se puede usar si los power-ups están bloqueados

---

## ?? **Cuándo Usar el Cambio de Pregunta**

### **USAR en:**
- ?? Preguntas donde tienes **dudas entre 2-3 opciones**
- ?? Cuando el temporizador se está acabando
- ?? Nivel 3 (preguntas difíciles, 10 preguntas totales)
- ?? Cuando quieres **proteger tu racha**
- ?? Cuando no estás seguro de la respuesta

### **NO USAR en:**
- ? Preguntas donde **estás seguro** de la respuesta
- ? Jefe Final (no disponible)
- ? Como "preventivo" en preguntas fáciles
- ? Al inicio del juego (mejor guardarlo para después)

---

## ?? **Distribución de Preguntas en la Base de Datos**

| Dificultad | Preguntas Disponibles | Usadas por Nivel |
|------------|----------------------|------------------|
| 1 (Fácil) | 6 | Nivel 1: 5 + 1 reserva |
| 2 (Media) | 8 | Nivel 2: 7 + 1 reserva |
| 3 (Difícil) | 7 | Nivel 3: 10 (¿necesita más?) |
| 4 (Jefe) | 3 | Nivel 4: 3 (sin reserva) |

---

**¡El Cambio de Pregunta es tu salvavidas! Úsalo sabiamente y solo cuando realmente lo necesites. ??**

## ?? Descripción

El power-up **Reintento** permite al jugador volver a responder una pregunta si falla en su primer intento.

## ?? Funcionamiento Completo

### **1. Activación del Power-Up**

Cuando el jugador hace clic en el botón **?? Reintento**:

```csharp
// GameManager.cs línea 89-90
case PowerUpType.Retry:
    IsRetryAvailable = true;  // Se activa la bandera
    break;
```

- ? Se consume 1 power-up del inventario del jugador
- ? Se activa la bandera `IsRetryAvailable = true`
- ? El jugador ahora tiene protección para el siguiente error

### **2. Respuesta Incorrecta CON Reintento**

Si el jugador responde incorrectamente Y tiene `IsRetryAvailable = true`:

```csharp
// GameForm.cs línea 172-183
if (_gameManager.IsRetryAvailable)
{
    MessageBox.Show("? Incorrecto. ¡Tienes un REINTENTO disponible!\n\nIntenta de nuevo.");
    
    // Desactivar el reintento
    _gameManager.ConsumeRetry();
    
    // Restaurar el botón y reiniciar temporizador
    button.BackColor = Color.FromArgb(33, 150, 243);
    UpdateUI();
    StartTimer();
}
```

**Comportamiento:**
- ? **NO** se aplica la penalización de -25 puntos
- ? **NO** se reinicia la racha de aciertos
- ? **NO** se aplican penalizaciones aleatorias
- ? **NO** se avanza a la siguiente pregunta
- ? **SÍ** se restaura el temporizador
- ? **SÍ** se restauran los colores de los botones
- ? **SÍ** se puede volver a responder

### **3. Respuesta Incorrecta SIN Reintento**

Si el jugador responde incorrectamente Y NO tiene reintento:

```csharp
// GameForm.cs línea 185-200
else
{
    MessageBox.Show($"? Incorrecto. La respuesta correcta era: {correcta}");
    
    // Aplicar penalizaciones normales
    // Avanzar a la siguiente pregunta
}
```

**Comportamiento:**
- ? Se aplica la penalización de -25 puntos
- ? Se reinicia la racha de aciertos a 0
- ? Se aplican penalizaciones aleatorias
- ? Se avanza a la siguiente pregunta

### **4. Consumo del Reintento**

El método `ConsumeRetry()` desactiva la protección:

```csharp
// GameManager.cs
public void ConsumeRetry()
{
    IsRetryAvailable = false;  // Se desactiva la protección
}
```

---

## ?? **Flujo Completo del Reintento**

```
1. Usuario hace clic en "?? Reintento"
         ?
2. Se consume 1 power-up del inventario
         ?
3. IsRetryAvailable = true
         ?
4. Usuario responde una pregunta
         ?
   ¿Respuesta correcta?
         ?
   ?????????????
   SÍ         NO
   ?          ?
Avanzar   ¿Tiene reintento?
          ?
    ?????????????
    SÍ         NO
    ?          ?
Reintentar   Penalizar
    ?          y Avanzar
ConsumeRetry()
IsRetryAvailable = false
Resetear UI
Iniciar temporizador
```

---

## ?? **Características Importantes**

### **Protección Total**
Cuando está activo, el reintento protege de:
- ? Pérdida de puntos (-25)
- ? Pérdida de racha
- ? Penalizaciones aleatorias del sistema
- ? Avance forzado a la siguiente pregunta

### **Uso Único**
- ?? El reintento se consume **inmediatamente** al fallar
- ?? Solo protege **UN** error
- ?? Si fallas dos veces seguidas, el segundo error sí cuenta

### **Estrategia Recomendada**
1. Usar en preguntas difíciles
2. Usar cuando queden pocos segundos
3. Usar en el Jefe Final (Nivel 4)
4. **NO** usar en preguntas fáciles

---

## ?? **Ejemplo de Uso**

### **Caso 1: Uso Exitoso**
```
1. Nivel 3, Pregunta difícil
2. Usuario activa "?? Reintento"
3. Responde incorrectamente
4. Mensaje: "¡Tienes REINTENTO disponible!"
5. Temporizador se reinicia
6. Usuario lee mejor la pregunta
7. Responde correctamente
8. ¡Gana puntos sin penalización! ?
```

### **Caso 2: Doble Error**
```
1. Usuario activa "?? Reintento"
2. Primera respuesta incorrecta ? Protegido ?
3. Segunda respuesta incorrecta ? Penalizado ?
   - Pierde 25 puntos
   - Racha = 0
   - Avanza a siguiente pregunta
```

---

## ?? **Implementación Técnica**

### **Archivos Modificados:**

1. **GameForm.cs** - Lógica de reintento en `OptionButton_Click`
2. **GameManager.cs** - Modificación de `CheckAnswer` para no avanzar si hay reintento
3. **GameManager.cs** - Nuevo método `ConsumeRetry()`

### **Variables de Estado:**

```csharp
public bool IsRetryAvailable { get; set; }  // En GameManager
```

- `true`: Protección activa
- `false`: Sin protección

---

## ?? **Comparación: Con vs Sin Reintento**

| Aspecto | Sin Reintento | Con Reintento |
|---------|---------------|---------------|
| Respuesta incorrecta | -25 puntos | 0 puntos |
| Racha | Se reinicia a 0 | Se mantiene |
| Penalizaciones | Se aplican | NO se aplican |
| Avanzar pregunta | SÍ | NO |
| Intentos | 1 | 2 |

---

## ?? **Limitaciones**

1. ? Cantidad inicial: **2 usos**
2. ? Solo protege **1 error por activación**
3. ? Se debe activar **ANTES** de responder
4. ? No se puede usar si los power-ups están bloqueados
5. ? No se puede combinar con otros power-ups en la misma pregunta (según reglas)

---

## ?? **Cuándo Usar el Reintento**

### **USAR en:**
- ?? Preguntas difíciles (Nivel 3-4)
- ?? Cuando tienes dudas entre 2 opciones
- ?? Jefe Final (muy recomendado)
- ?? Cuando quieres mantener tu racha

### **NO USAR en:**
- ? Preguntas fáciles (Nivel 1-2)
- ? Cuando estás seguro de la respuesta
- ? Al inicio del juego (guárdalo para después)

---

**¡El Reintento es tu segunda oportunidad! Úsalo sabiamente. ??**
