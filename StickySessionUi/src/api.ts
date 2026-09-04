import axios from "axios";
import type { User } from "./types";

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL as string;

export const api = axios.create({
  baseURL: API_BASE_URL,
});

export async function fetchUsers(): Promise<User[]> {
  const response = await api.get<User[]>("/api/users");
  return response.data;
}

export async function sendMessage(userId: number, message: string): Promise<void> {
  await api.post("/api/messages", { userId, message });
}
