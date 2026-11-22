import apiClient from './client';
import type { EmotionDto, EmotionType } from '../types';

export const getSupportedEmotionTypes = async (): Promise<string[]> => {
  const response = await apiClient.get<string[]>('/emotions/types');
  return response.data;
};

export const getHumanEmotions = async (humanId: string): Promise<EmotionDto[]> => {
  const response = await apiClient.get<EmotionDto[]>(`/humans/${humanId}/emotions`);
  return response.data;
};

export const getEmotionByType = async (humanId: string, emotionType: EmotionType): Promise<EmotionDto> => {
  const response = await apiClient.get<EmotionDto>(`/humans/${humanId}/emotions/${emotionType}`);
  return response.data;
};

export const addEmotion = async (
  humanId: string,
  type: EmotionType,
  percentage: number
): Promise<EmotionDto> => {
  const response = await apiClient.post<EmotionDto>(`/humans/${humanId}/emotions`, {
    type,
    percentage,
  });
  return response.data;
};

export const updateEmotion = async (
  humanId: string,
  type: EmotionType,
  percentage: number
): Promise<EmotionDto> => {
  const response = await apiClient.put<EmotionDto>(`/humans/${humanId}/emotions/${type}`, {
    percentage,
  });
  return response.data;
};

export const deleteEmotion = async (humanId: string, type: EmotionType): Promise<void> => {
  await apiClient.delete(`/humans/${humanId}/emotions/${type}`);
};

