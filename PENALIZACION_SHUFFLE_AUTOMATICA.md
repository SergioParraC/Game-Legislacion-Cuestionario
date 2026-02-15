# ?? Penalización: Mezcla Automática de Opciones cada 7 segundos

## ?? Descripción

La penalización **ShuffleOptions** ahora hace que las opciones de respuesta se **mezclen automáticamente cada 7 segundos** mientras esté activa durante toda la pregunta.

---

## ?? Funcionamiento

### **Antes (Mezcla Única):**
```
?? Penalización aplicada
?
?? Opciones mezcladas UNA VEZ
?
? Usuario responde (sin más cambios)
```

### **Ahora (Mezcla Continua):**
```
?? Penalización aplicada
?
?? Opciones mezcladas inmediatamente
?
? 7 segundos después...
?
?? Opciones mezcladas nuevamente
?
? 7 segundos después...
?
?? Opciones mezcladas nuevamente
?
... (continúa hasta que el usuario responda o se acabe el tiempo)
```

---

## ?? Implementación Técnica

### **1. Nueva Propiedad en GameManager.cs**

```csharp
public bool IsShufflePenaltyActive { get; set; }
```

Esta bandera indica si la penalización de mezcla automática está activa.

### **2. Activación de la Penalización**

```csharp
private void ApplyRandomPenalty()
{
    switch (Penality)
    {
        case PenaltyType.ShuffleOptions:
            IsShufflePenaltyActive = true;  // ? Activar bandera
            ShuffleOptions();
            LastPenaltyMessage = "?? ¡Penalización! Opciones se mezclarán cada 7 segundos";
            break;
    }
}
```

### **3. Desactivación al Cargar Nueva Pregunta**

```csharp
public void LoadNextQuestion()
{
    IsShufflePenaltyActive = false;  // ? Desactivar al cambiar de pregunta
    // ... resto del código
}
```

### **4. Timer de Mezcla en GameForm.cs**

```csharp
private System.Windows.Forms.Timer _shuffleTimer;

// Inicialización
_shuffleTimer = new System.Windows.Forms.Timer { Interval = 7000 };
_shuffleTimer.Tick += ShuffleTimer_Tick;
```

### **5. Evento de Mezcla Automática**

```csharp
private void ShuffleTimer_Tick(object sender, EventArgs e)
{
    if (_gameManager.IsShufflePenaltyActive)
    {
        _gameManager.ShuffleCurrentOptions();
        UpdateOptionsDisplay();
        
        lblPenalty.Text = "?? ¡Opciones mezcladas! (Penalización activa)";
        lblPenalty.ForeColor = Color.FromArgb(255, 87, 34);
    }
    else
    {
        _shuffleTimer.Stop();
    }
}
```

### **6. Control del Timer**

```csharp
private void StartTimer()
{
    _timer.Start();
    
    // Solo iniciar si la penalización está activa
    if (_gameManager.IsShufflePenaltyActive)
    {
        _shuffleTimer.Start();
    }
}
```

---

## ?? Ejemplo de Comportamiento

### **Escenario: Nivel 2, Penalización Aplicada**

```
?? Inicio (12 segundos)
?? ¡Penalización ShuffleOptions activada!

?? Opciones iniciales:
   [A] Madrid
   [B] Barcelona
   [C] Valencia
   [D] Sevilla

?? Tiempo: 5s (después de 7 segundos)
?? ¡Primera mezcla automática!
?? Nuevas posiciones:
   [A] Valencia
   [B] Madrid
   [C] Sevilla
   [D] Barcelona

?? Tiempo: 0s (el usuario no respondió a tiempo)
? Se acabó el tiempo
```

### **Escenario: Nivel 3, Penalización Aplicada**

```
?? Inicio (10 segundos)
?? ¡Penalización ShuffleOptions activada!

?? Opciones iniciales:
   [A] Júpiter
   [B] Marte
   [C] Saturno
   [D] Neptuno

?? Tiempo: 3s (después de 7 segundos)
?? ¡Primera mezcla automática!
?? Nuevas posiciones:
   [A] Marte
   [B] Neptuno
   [C] Júpiter
   [D] Saturno

? Usuario responde en 2 segundos
   (Timer se detiene antes de segunda mezcla)
```

---

## ?? Impacto en la Dificultad

| Tiempo Total | Penalización Activa | Mezclas Posibles |
|--------------|---------------------|------------------|
| 15s (Nivel 1) | ? | 2 mezclas |
| 12s (Nivel 2) | ? | 1 mezcla |
| 10s (Nivel 3) | ? | 1 mezcla |
| 7s (Jefe) | ? | 1 mezcla al inicio |

### **Comparación con Mezcla Normal:**

| Aspecto | Sin Penalización | Con Penalización Shuffle |
|---------|------------------|--------------------------|
| Mezclas | 0 | Cada 7 segundos |
| Dificultad | Normal | ?? Alta |
| Presión | Media | ???? Muy Alta |
| Estrategia | Leer y responder | ?? Responder RÁPIDO |

---

## ?? Estrategias del Jugador

### ? **Qué Hacer:**
1. **Leer TODAS las opciones** inmediatamente
2. **Memorizar la RESPUESTA CORRECTA** (no la posición)
3. **Responder RÁPIDO** antes de la primera mezcla (7 segundos)
4. **Usar Power-Up +5s** solo si es absolutamente necesario (causará más mezclas)

### ? **Qué NO Hacer:**
1. ? Memorizar solo la **posición** (A, B, C, D)
2. ? Dudar demasiado tiempo
3. ? Usar power-up de eliminar opción (las opciones restantes seguirán mezclándose)
4. ? Perder el foco cuando aparece el mensaje de mezcla

---

## ?? Interacción con Power-Ups

### **Power-Up: Eliminar Opción + Penalización Shuffle**

```
Estado inicial (con penalización):
[A] Opción incorrecta
[B] Opción correcta
[C] Opción incorrecta
[D] Opción incorrecta

Usuario usa "Eliminar Opción":
[A] Opción incorrecta
[B] Opción correcta
[C] Opción incorrecta
(Opción D eliminada)

? 7 segundos después (penalización sigue activa):
[A] Opción correcta    ? ¡Las 3 opciones restantes se mezclan!
[B] Opción incorrecta
[C] Opción incorrecta

?? La opción eliminada NO reaparece, pero las restantes siguen mezclándose
```

### **Power-Up: +5 Segundos + Penalización Shuffle**

```
Sin power-up: 12s ? 1 mezcla posible
Con power-up: 12s + 5s = 17s ? 2 mezclas posibles

?? Usar +5 segundos con esta penalización es RIESGOSO
   ? Más tiempo = MÁS mezclas = MÁS confusión
```

---

## ?? Probabilidad de Activación

| Nivel | Probabilidad | Frecuencia Esperada |
|-------|--------------|---------------------|
| 1 | 0% | Nunca |
| 2 | 20% | ~1-2 veces por nivel |
| 3 | 40% | ~4 veces por nivel |
| 4 (Jefe) | 100% | Siempre (en pregunta final) |

---

## ?? Indicadores Visuales

### **Mensaje al Aplicarse:**
```
???????????????????????????????????????????????????????
? ?? ¡Penalización! Opciones se mezclarán cada 7s    ?
? Color: Naranja (#FF9800)                            ?
???????????????????????????????????????????????????????
```

### **Mensaje en Cada Mezcla:**
```
???????????????????????????????????????????????????????
? ?? ¡Opciones mezcladas! (Penalización activa)      ?
? Color: Naranja Oscuro (#FF5722)                    ?
???????????????????????????????????????????????????????
```

---

## ?? Casos Edge

### **Caso 1: Usuario responde antes de 7 segundos**
```
? Timer de mezcla se detiene
? No hay mezclas adicionales
? Comportamiento normal
```

### **Caso 2: Power-Up "Cambio de Pregunta" usado**
```
? Nueva pregunta se carga
? IsShufflePenaltyActive = false
? Timer de mezcla se detiene
? Nueva pregunta comienza sin penalización (a menos que se aplique nuevamente)
```

### **Caso 3: Nivel completado durante penalización**
```
? Timer de mezcla se detiene
? Penalización no se transfiere al siguiente nivel
? Siguiente nivel comienza limpio
```

### **Caso 4: Se acaba el tiempo durante penalización**
```
? Timer de mezcla se detiene
? Usuario pierde puntos y racha
? Nueva pregunta comienza sin penalización
```

---

## ?? Control de la Penalización

### **Activación:**
- ? Aleatoria en Nivel 2+ (20-40% probabilidad)
- ? Manual si usuario falla y se aplica penalización aleatoria

### **Desactivación:**
- ? Al cargar nueva pregunta (`LoadNextQuestion()`)
- ? Al responder (correcto o incorrecto)
- ? Al acabarse el tiempo
- ? Al completar nivel
- ? Al terminar juego

---

## ?? Archivos Modificados

### **1. Game/Managers/GameManager.cs**
- ? Agregada propiedad `IsShufflePenaltyActive`
- ? Modificado `ApplyRandomPenalty()` para activar la bandera
- ? Modificado `LoadNextQuestion()` para desactivar la bandera
- ? Agregado método público `ShuffleCurrentOptions()`

### **2. Game/Forms/GameForm.cs**
- ? Agregado `_shuffleTimer`
- ? Método `ShuffleTimer_Tick()` para mezcla automática
- ? Método `UpdateOptionsDisplay()` separado
- ? Control de inicio/parada del timer basado en penalización

---

## ?? Beneficios de esta Implementación

1. **Penalización real**: Ya no es una mezcla única, sino continua
2. **Mayor dificultad**: El jugador debe responder rápido o sufrir múltiples mezclas
3. **Estratégico**: Fomenta decisiones rápidas
4. **Presión temporal**: Se suma a la presión del temporizador principal
5. **Diferenciación de niveles**: Niveles altos tienen más probabilidad de esta penalización

---

## ?? Configuración

Para cambiar el intervalo de mezcla:

```csharp
// En GameForm.cs
_shuffleTimer = new System.Windows.Forms.Timer { Interval = 5000 }; // 5 segundos
_shuffleTimer = new System.Windows.Forms.Timer { Interval = 10000 }; // 10 segundos
```

Para cambiar la probabilidad de activación:

```csharp
// En GameManager.cs - ApplyLevelPenalties()
if (CurrentLevel.Number >= 2 && _random.Next(100) < 30)  // 30% en nivel 2
if (CurrentLevel.Number >= 3 && _random.Next(100) < 50)  // 50% en nivel 3
```

---

## ? Resumen

Esta penalización convierte el juego en un desafío de **memoria + velocidad**:

- ?? Debes **memorizar** la respuesta correcta
- ? Debes **responder rápido** antes de perder la orientación
- ?? Debes **mantener el foco** a pesar de las mezclas continuas

**¡La penalización ShuffleOptions es ahora una de las más desafiantes del juego! ??**
