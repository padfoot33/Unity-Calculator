# Unity DMAS Calculator (WebGL)

A lightweight calculator built in Unity that evaluates long expressions using **DMAS/BODMAS** (Multiplication/Division before Addition/Subtraction, left→right). It ships with a custom expression parser/evaluator—no built‑in evaluators or third‑party libraries—built specifically for the GameBee Studio assignment.

**Live demo:**  
- Itch.io: https://padfoot007.itch.io/unity-calculator  

**Source repo:** https://github.com/padfoot33/Unity-Calculator

---

## Features

- `+  −  ×  ÷` with correct **operator precedence** (DMAS).
- Long expression support with decimals and **unary minus** (`-3*4`, `12+-5`, `7*-2`).
- **C** (clear last) and **AC** (reset all).
- Clean UI with TextMeshPro; mouse/touch input plus **keyboard & numpad** via Unity’s New Input System.
- Compact, readable, fully self-contained evaluator.

---

## Controls

**Mouse/Touch**  
Tap the on-screen keypad.

**Keyboard (New Input System)**  
- Digits: `0–9` (top row) and `Numpad 0–9`  
- Dot: `.` or `Numpad .`  
- Operators: `Numpad +  -  *  /` (top-row `-` also mapped)  
- Equals: `Enter` / `Numpad Enter`  
- Clear last: `Backspace`  
- Reset all: `Esc` or `Delete`

> Note: Holding one digit while tapping another might ignore the second on some keyboards; the current input map favors simplicity over per-key concurrency. See the Input section if you want to switch to Pass-Through bindings or text input for stricter behavior.

---

## How it works (high level)

1. **Tokenize** the input string into numbers and operators, correctly handling **unary minus** (e.g., `3*-2` → `3`, `*`, `-2`).  
2. **Pass 1 (×, ÷)**: scan left→right and immediately fold multiplications/divisions into a reduced token list.  
3. **Pass 2 (+, −)**: fold the reduced list left→right to produce the final result.  
4. Invalid inputs (double dots, dangling operators, divide by zero, unknown symbols) return `"Error"` in the UI.

No `DataTable.Compute`, no third‑party expression libraries, and no Unity internal `ExpressionEvaluator`—the evaluator is custom C#.

---


## Getting started

- Unity **2022.x LTS** recommended (recent LTS versions also work).  
- Open the project → open the main calculator scene → **Play**.

If keyboard doesn’t respond:  
- Project Settings → **Player → Active Input Handling** = **Input System Package (New)** (or **Both**).  
- Ensure there’s an **EventSystem** with **Input System UI Input Module**.

---


## Edge cases handled

- Trailing operator (trims once): `"5+"` → `"5"`; still-invalid inputs error.  
- Consecutive operators: UI prevents/normalizes; tokenizer validates alternating `Number/Op`.  
- Unary minus: `-3*4`, `12+-5`, `7*-2`.  
- Decimals: `.5+1.5`, `5.`, `3..4` (error).  
- Divide by zero: error.  
- Invalid characters: error.  
- Locale-independent parsing/formatting (`InvariantCulture`).

---

## Tech

- Unity, TextMeshPro, New Input System  
- WebGL (Brotli/Gzip)  
- No runtime dependencies beyond Unity

---

## Known limitations

- Parentheses and advanced ops (`^`, `%`, `sqrt`, etc.) are **out of scope** by design.  
- Extremely large values may overflow to `Infinity` (treated as error for display).

---

## License

MIT

---

## Credits

Built by **Param** for the GameBee Studio assignment.  
Repo maintainer: [@padfoot33](https://github.com/padfoot33)
