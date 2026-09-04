export interface User {
  id: number;
  username: string;
}

export interface ChatMessage {
  fromUserId?: number;
  message: string;
  receivedAt: string;
}
