import apiClient from './client';
import type { HumanDto, CreateHumanRequest, UpdateHumanRequest } from '../types';

export const getHumans = async (): Promise<HumanDto[]> => {
  const response = await apiClient.get<HumanDto[]>('/humans');
  return response.data;
};

export const getHumanById = async (id: string): Promise<HumanDto> => {
  const response = await apiClient.get<HumanDto>(`/humans/${id}`);
  return response.data;
};

export const createHuman = async (request: CreateHumanRequest): Promise<HumanDto> => {
  const response = await apiClient.post<HumanDto>('/humans', request);
  return response.data;
};

export const updateHuman = async (id: string, request: UpdateHumanRequest): Promise<HumanDto> => {
  const response = await apiClient.put<HumanDto>(`/humans/${id}`, request);
  return response.data;
};

export const deleteHuman = async (id: string): Promise<void> => {
  await apiClient.delete(`/humans/${id}`);
};
