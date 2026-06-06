import axios from 'axios';

export const apiClient = axios.create({
  baseURL: 'http://localhost:5279/api',
  timeout: 5000,
});