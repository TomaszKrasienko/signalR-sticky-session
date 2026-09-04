import { useEffect, useState } from "react";
import { fetchUsers, sendMessage } from "./api";
import { useMessagingHub } from "./useMessagingHub";
import type { User } from "./types";
import "./App.css";

function App() {
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUserId, setSelectedUserId] = useState<number | "">("");
  const [messageUserId, setMessageUserId] = useState<number | "">("");
  const [messageText, setMessageText] = useState("");
  const [isSending, setIsSending] = useState(false);

  const { connect, isConnected, messages } = useMessagingHub();

  useEffect(() => {
    fetchUsers()
      .then(setUsers)
      .catch((err) => console.error("Failed to load users", err));
  }, []);

  const handleConnect = async () => {
    if (selectedUserId === "") return;
    try {
      await connect(selectedUserId);
    } catch (err) {
      console.error("Failed to connect to hub", err);
    }
  };

  const handleSend = async () => {
    if (messageUserId === "" || !messageText.trim()) return;
    setIsSending(true);
    try {
      await sendMessage(messageUserId, messageText);
      setMessageText("");
    } catch (err) {
      console.error("Failed to send message", err);
    } finally {
      setIsSending(false);
    }
  };

  return (
    <div className="app-layout">
      <div className="left-panel">
        <h2>Messaging</h2>

        <div className="field">
          <label>User (select)</label>
          <select
            value={selectedUserId}
            onChange={(e) => setSelectedUserId(Number(e.target.value))}
          >
            <option value="">-- select user --</option>
            {users.map((u) => (
              <option key={u.id} value={u.id}>
                {u.username}
              </option>
            ))}
          </select>
        </div>

        <button onClick={handleConnect} disabled={isConnected || selectedUserId === ""}>
          {isConnected ? "Connected" : "Połącz"}
        </button>

        <div className="field">
          <label>Wiadomość</label>
          <input
            type="text"
            value={messageText}
            onChange={(e) => setMessageText(e.target.value)}
            placeholder="Treść wiadomości"
          />
        </div>

        <div className="field">
          <label>Odbiorca</label>
          <select
            value={messageUserId}
            onChange={(e) => setMessageUserId(Number(e.target.value))}
          >
            <option value="">-- select user --</option>
            {users.map((u) => (
              <option key={u.id} value={u.id}>
                {u.username}
              </option>
            ))}
          </select>
        </div>

        <button onClick={handleSend} disabled={isSending}>
          Wyślij
        </button>
      </div>

      <div className="right-panel">
        <h2>Odebrane wiadomości</h2>
        <ul className="messages-list">
          {messages.map((m, idx) => (
            <li key={idx}>
              <span className="message-from">
                {m.fromUserId !== undefined ? `User ${m.fromUserId}` : "?"}
              </span>
              <span className="message-text">{m.message}</span>
              <span className="message-time">
                {new Date(m.receivedAt).toLocaleTimeString()}
              </span>
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}

export default App;
