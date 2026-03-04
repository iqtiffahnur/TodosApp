import { useEffect, useState } from "react";
const API_BASE = import.meta.env.VITE_API_BASE_URL ?? "";
//fetch(`${API_BASE}/api/todos`);
import "./index.css";

console.log("API_BASE =", API_BASE);

async function readJsonOrThrow(res, defaultMsg) {
  const ct = res.headers.get("content-type") || "";
  if (!ct.includes("application/json")) {
    const txt = await res.text();
    throw new Error(`${defaultMsg}. Server returned non-JSON:\n${txt.slice(0, 140)}...`);
  }
  return res.json();
}

function AppBar({ onToggleTheme, theme }) {
  return (
    <header className="appbar">
      <div className="appbar-title">Todo Manager</div>
      <button className="icon-btn" onClick={onToggleTheme} aria-label="Toggle theme">
        {theme === "light" ? "🌙" : "☀️"}
      </button>
    </header>
  );
}

export default function App() {
  // THEME
  const [theme, setTheme] = useState(
    () => localStorage.getItem("theme") || "light"
  );
  useEffect(() => {
    document.documentElement.setAttribute("data-theme", theme);
    localStorage.setItem("theme", theme);
  }, [theme]);
  const toggleTheme = () => setTheme(t => (t === "light" ? "dark" : "light"));

  // DATA
  const [todos, setTodos] = useState([]);
  const [title, setTitle] = useState("");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState("");

  async function load() {
    try {
      setLoading(true);
      setError("");
      const res = await fetch(`${API_BASE}/api/todos`, { cache: "no-store" });
      if (!res.ok) throw new Error("Failed to load todos");
      const data = await readJsonOrThrow(res, "Failed to load todos");
      setTodos(data);
    } catch (e) {
      setError(e.message || "Failed to fetch");
    } finally {
      setLoading(false);
    }
  }
  useEffect(() => { load(); }, []);

  async function addTodo(e) {
    e.preventDefault();
    const text = title.trim();
    if (!text) return;
    try {
      setSaving(true);
      setError("");
      const res = await fetch(`${API_BASE}/api/todos`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: text }),
        cache: "no-store"
      });
      if (!res.ok) {
        let msg = "Failed to add";
        try {
          const data = await readJsonOrThrow(res, "Failed to add todo");
          msg = data?.error || msg;
        } catch {
          msg = (await res.text()) || msg;
        }
        throw new Error(msg);
      }
      setTitle("");
      await load();
    } catch (e) {
      setError(e.message || "Failed to fetch");
    } finally {
      setSaving(false);
    }
  }

  async function toggleDone(todo) {
    try {
      setError("");
      await fetch(`${API_BASE}/api/todos/${todo.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: todo.title, isCompleted: !todo.isCompleted })
      });
      await load();
    } catch (e) {
      setError(e.message);
    }
  }

  async function remove(id) {
    try {
      setError("");
      const res = await fetch(`${API_BASE}/api/todos/${id}`, {
        method: "DELETE",
        cache: "no-store"
      });
      if (!res.ok) throw new Error("Failed to delete");
      await load();
    } catch (e) {
      setError(e.message || "Failed to fetch");
    }
  }

  return (
    <>
      <AppBar onToggleTheme={toggleTheme} theme={theme} />

      <main className="shell">
        <section className="card">
          <h1>Tasks</h1>
          <p className="subtle">Plan your work and track progress.</p>

          <form className="input-row" onSubmit={addTodo}>
            <div className="input-control">
              <input
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                placeholder="What do you need to do?"
              />
            </div>
            <button className="add-btn" disabled={saving}>
              <span className="plus">+</span>
              Add
            </button>
          </form>

          {error && (
            <div className="empty" style={{ borderStyle: "solid", color: "#ef4444" }}>
              {error}
            </div>
          )}

          {loading ? (
            <div className="empty">Loading to-dos…</div>
          ) : todos.length === 0 ? (
            <div className="empty">
              <div style={{ fontWeight: 700, marginBottom: 6 }}>No to-dos yet</div>
              <div>Add your first task using the field above.</div>
            </div>
          ) : (
            <div className="list">
              {todos.map((t) => (
                <div key={t.id} className={`item ${t.isCompleted ? "done" : ""}`}>
                  <input
                    type="checkbox"
                    checked={t.isCompleted}
                    onChange={() => toggleDone(t)}
                  />
                  <div className="item-title">{t.title}</div>
                  <button className="delete-btn" onClick={() => remove(t.id)}>
                    Delete
                  </button>
                </div>
              ))}
            </div>
          )}
        </section>
      </main>
    </>
  );
}