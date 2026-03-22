import type { AuthUser } from '@/types';

export const mockUsers: AuthUser[] = [
  { id: 'u1', name: 'Alice Admin', email: 'alice@titan.lib', role: 'admin' , token:''},
  { id: 'u2', name: 'Bob Admin', email: 'bob@titan.lib', role: 'admin' , token:''},
  { id: 'u3', name: 'Carol Customer', email: 'carol@titan.lib', role: 'customer' , token:''},
  { id: 'u4', name: 'Dave Customer', email: 'dave@titan.lib', role: 'customer' , token:''},
  { id: 'u5', name: 'Eve Customer', email: 'eve@titan.lib', role: 'customer' , token:''},
  { id: 'u6', name: 'Frank Customer', email: 'frank@titan.lib', role: 'customer' , token:''},
  { id: 'u7', name: 'Grace Customer', email: 'grace@titan.lib', role: 'customer' , token:''},
  { id: 'u8', name: 'Henry Author', email: 'henry@titan.lib', role: 'author' , token:''},
  { id: 'u9', name: 'Iris Author', email: 'iris@titan.lib', role: 'author' , token:''},
  { id: 'u10', name: 'Jack Author', email: 'jack@titan.lib', role: 'author' , token:''},
];
