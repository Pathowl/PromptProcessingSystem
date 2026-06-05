import { useState, useEffect } from "react";
import axios from "axios";

const API_URL = "http://localhost:5279/api/prompts";

function App() {
  const [content, setContent] = useState("");
  const [prompts, setPrompts] = useState([]);

  // prompt list
  const fetchPrompts = async () => {
    const response = await axios.get(API_URL);
    setPrompts(response.data);
  };

  // polling (avoiding infinite waiting on backend and constant errors without backend running)
useEffect(() => {
  const fetchData = async () => {
    try {
      const response = await axios.get("http://localhost:5279/api/prompts", {
        timeout: 2000 // Ustaw timeout, żeby nie czekać w nieskończoność
      });
      setPrompts(response.data);
    } catch (error) {
      // checking if error is a network error (like backend not running) and ignoring it to avoid console spam
      if (error.code === 'ERR_CONNECTION_REFUSED' || !error.response) {
        // Backend is not running or not reachable, we can ignore this error in the console
        return; 
      }
      // For other types of errors (like 500, 404, etc.) we log them to the console for debugging purposes
      console.error("Wystąpił nieoczekiwany błąd:", error);
    }
  };

  fetchData();
  const interval = setInterval(fetchData, 1000);
  
  return () => clearInterval(interval);
}, []);

  // new prompt
  const handleSubmit = async (e) => {
    e.preventDefault();
    await axios.post(API_URL, { content });
    setContent(""); //clean input after submit
  };

  return (
    <div style={{ padding: "20px" }}>
      <h1>AI Prompt Manager</h1>
      <form onSubmit={handleSubmit}>
        <input
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Wpisz prompt..."
        />
        <button type="submit">Dodaj prompt</button>
      </form>

      <table border="1" style={{ marginTop: "20px", width: "100%" }}>
        <thead>
          <tr>
            <th>Content</th>
            <th>Status</th>
            <th>Result</th>
          </tr>
        </thead>
        <tbody>
          {prompts.map((p) => (
            <tr key={p.id}>
              <td>{p.content}</td>
              <td>{p.status}</td>
              <td>{p.result || p.errorMessage || "-"}</td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default App;
