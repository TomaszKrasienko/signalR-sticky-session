import { useCallback, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";
import type { ChatMessage } from "./types";

const API_BASE_URL = (import.meta.env.VITE_API_BASE_URL as string) ?? "";

export function useMessagingHub() {
  const connectionRef = useRef<signalR.HubConnection | null>(null);
  const [isConnected, setIsConnected] = useState(false);
  const [messages, setMessages] = useState<ChatMessage[]>([]);

  const connect = useCallback(async (userId: number) => {
    if (connectionRef.current) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${API_BASE_URL}/hubs/messaging?userId=${userId}`)
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveMessage", (fromUserId: number, message: string) => {
      setMessages((prev) => [
        ...prev,
        { fromUserId, message, receivedAt: new Date().toISOString() },
      ]);
    });

    connection.onclose(() => setIsConnected(false));

    await connection.start();
    connectionRef.current = connection;
    setIsConnected(true);
  }, []);

  return { connect, isConnected, messages };
}
