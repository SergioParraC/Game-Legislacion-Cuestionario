# ?? Documentación: Mezcla Automática de Opciones

## ?? Descripción

El juego ahora incluye una **mecánica de dificultad adicional**: Las opciones de respuesta se **mezclan automáticamente cada 7 segundos** durante el tiempo de respuesta.

---

## ?? Funcionamiento

### **Timer de Mezcla Automática**

```
?? Inicio de pregunta
    ?
?? Opciones en posiciones A, B, C, D
    ?
? 7 segundos después...
    ?
?? Opciones mezcladas: C, A, D, B
    ?
? 7 segundos después...
    ?
?? Opciones mezcladas nuevamente: B, D, A, C
    ?
? Usuario responde o se acaba el tiempo
```

---

## ?? Implementación Técnica

### **1. Nuevo Timer en GameForm.cs**

```csharp
private System.Windows.Forms.Timer _shuffleTimer;

// Inicialización
_shuffleTimer = new System.Windows.Forms.Timer { Interval = 7000 }; // 7 segundos
_shuffleTimer.Tick += ShuffleTimer_Tick;
```

### **2. Método de Mezcla Automática**

```csharp
private void ShuffleTimer_Tick(object sender, EventArgs e)
{
    // Mezclar las opciones automáticamente cada 7 segundos
    _gameManager.ShuffleCurrentOptions();
    UpdateOptionsDisplay();
    
    // Mostrar indicador visual de mezcla
    lblPenalty.Text = "?? ¡Opciones mezcladas automáticamente!";
    lblPenalty.ForeColor = Color.FromArgb(255, 152, 0);
}
```

### **3. Método Público en GameManager.cs**

```csharp
public void ShuffleCurrentOptions()
{
    // Método público para mezclar las opciones actuales desde el formulario
    DisplayedOptions = [.. DisplayedOptions.OrderBy(x => _random.Next())];
}
```

### **4. Control del Timer**

El timer se **inicia** cuando:
```csharp
private void StartTimer()
{
    _timer.Start();
    _shuffleTimer.Start(); // ? Inicia junto con el timer principal
}
```

El timer se **detiene** cuando:
```csharp
// Al responder
_timer.Stop();
_shuffleTimer.Stop();

// Al acabarse el tiempo
_timer.Stop();
_shuffleTimer.Stop();

// Al completar nivel
_timer.Stop();
_shuffleTimer.Stop();

// Al terminar juego
_timer.Stop();
_shuffleTimer.Stop();
```

---

## ?? Ejemplo de Comportamiento

### **Caso 1: Pregunta con 15 segundos**

```
?? Tiempo: 15s
?? Opciones:
   [A] Madrid
   [B] Barcelona
   [C] Valencia
   [D] Sevilla

?? Tiempo: 8s (después de 7 segundos)
?? ¡Opciones mezcladas!
?? Nuevas posiciones:
   [A] Valencia
   [B] Madrid
   [C] Sevilla
   [D] Barcelona

?? Tiempo: 1s (después de 14 segundos)
?? ¡Opciones mezcladas de nuevo!
?? Nuevas posiciones:
   [A] Sevilla
   [B] Valencia
   [C] Madrid
   [D] Barcelona

? Usuario responde: "Madrid" (opción C)
```

### **Caso 2: Pregunta con 10 segundos**

```
?? Tiempo: 10s
?? Opciones:
   [A] Júpiter
   [B] Marte
   [C] Saturno
   [D] Neptuno

?? Tiempo: 3s (después de 7 segundos)
?? ¡Opciones mezcladas!
?? Nuevas posiciones:
   [A] Marte
   [B] Neptuno
   [C] Júpiter
   [D] Saturno

? Usuario responde antes de la segunda mezcla
```

---

## ?? Impacto en el Juego

### **Aumento de Dificultad**

| Nivel | Tiempo | Mezclas Posibles |
|-------|--------|------------------|
| 1 | 15s | Hasta 2 mezclas |
| 2 | 12s | Hasta 1 mezcla |
| 3 | 10s | Hasta 1 mezcla |
| Jefe | 7s | 1 mezcla al inicio |

### **Estrategias del Jugador**

#### ? **Recomendaciones:**
1. **Leer todas las opciones rápidamente** al inicio
2. **Memorizar la respuesta correcta** en lugar de su posición
3. **Responder rápido** si estás seguro (antes de la primera mezcla)
4. **Usar el power-up de +5 segundos** si necesitas más tiempo sin mezclas adicionales

#### ?? **Errores Comunes:**
1. ? Memorizar solo la **posición** (A, B, C, D) en lugar del **contenido**
2. ? Dudar demasiado tiempo (más mezclas = más confusión)
3. ? Perder el foco cuando aparece el mensaje "?? ¡Opciones mezcladas!"

---

## ?? Interacción con Power-Ups

### **Power-Up: Eliminar Opción**
```
Antes de la mezcla:
[A] Opción incorrecta ? Eliminada
[B] Opción correcta
[C] Opción incorrecta
[D] Opción incorrecta

Después de la mezcla (7 segundos):
[A] Opción correcta
[B] Opción incorrecta
[C] Opción incorrecta
(La opción eliminada sigue eliminada)
```

### **Power-Up: +5 Segundos**
```
Sin power-up: 15s ? 2 mezclas posibles
Con power-up: 15s + 5s = 20s ? 2-3 mezclas posibles

?? Usar +5 segundos puede causar más mezclas!
```

---

## ?? Indicador Visual

Cuando las opciones se mezclan automáticamente, aparece:

```
???????????????????????????????????????????????
? ?? ¡Opciones mezcladas automáticamente!     ?
? Color: Naranja (#FF9800)                    ?
? Ubicación: Label de Penalizaciones          ?
???????????????????????????????????????????????
```

Este mensaje se muestra temporalmente y se sobrescribe con:
- Mensajes de penalizaciones del sistema
- Mensajes vacíos al cargar nueva pregunta

---

## ?? Casos Edge

### **Caso 1: Uso de Power-Up "Eliminar Opción"**
- ? Las opciones restantes se mezclan correctamente
- ? La opción eliminada no reaparece

### **Caso 2: Cambio de Pregunta (Power-Up Reintento)**
- ? El timer de mezcla se reinicia
- ? Las opciones de la nueva pregunta se mezclan desde el inicio

### **Caso 3: Nivel con 3 opciones**
- ? La mezcla funciona con cualquier número de opciones
- ? Funciona con 2, 3 o 4 opciones

### **Caso 4: Respuesta al segundo 6.9**
- ? El timer se detiene inmediatamente al hacer clic
- ? No ocurre una mezcla mientras se procesa la respuesta

---

## ?? Estadísticas de Mezcla

| Tiempo Total | Intervalo | Mezclas Totales |
|--------------|-----------|-----------------|
| 7s o menos | 7s | 0 (sin mezclas) |
| 8-14s | 7s | 1 mezcla |
| 15-21s | 7s | 2 mezclas |
| 22-28s | 7s | 3 mezclas |

### **Por Nivel:**

```
Nivel 1 (15s): 
?? Primera mezcla: a los 7s
?? Segunda mezcla: a los 14s

Nivel 2 (12s):
?? Primera mezcla: a los 7s

Nivel 3 (10s):
?? Primera mezcla: a los 7s

Jefe Final (7s):
?? Sin mezclas automáticas (tiempo insuficiente)
```

---

## ?? Configuración

Para ajustar el intervalo de mezcla, modificar en `GameForm.cs`:

```csharp
// Cambiar de 7 segundos a otro valor
_shuffleTimer = new System.Windows.Forms.Timer { Interval = 5000 }; // 5 segundos
_shuffleTimer = new System.Windows.Forms.Timer { Interval = 10000 }; // 10 segundos
```

---

## ?? Beneficios de esta Mecánica

1. **Aumenta la dificultad** sin cambiar las preguntas
2. **Fomenta la lectura cuidadosa** en lugar de memorizar posiciones
3. **Agrega presión temporal** adicional
4. **Hace el juego más dinámico** visualmente
5. **Penaliza la indecisión** del jugador

---

## ?? Notas de Desarrollo

### **Archivos Modificados:**
1. ? `Game/Forms/GameForm.cs` - Agregado `_shuffleTimer` y eventos
2. ? `Game/Managers/GameManager.cs` - Agregado método público `ShuffleCurrentOptions()`

### **Nuevos Métodos:**
1. `ShuffleTimer_Tick()` - Maneja el evento de mezcla automática
2. `UpdateOptionsDisplay()` - Actualiza solo las opciones (sin toda la UI)
3. `ShuffleCurrentOptions()` - Mezcla las opciones actuales (público)

---

**¡Esta mecánica hace el juego mucho más desafiante y emocionante! ??**
