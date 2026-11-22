import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import {
  getHumanEmotions,
  getSupportedEmotionTypes,
  addEmotion,
  updateEmotion,
  deleteEmotion,
} from '../api/emotions';
import type { EmotionType } from '../types';

export const useEmotionTypes = () => {
  return useQuery({
    queryKey: ['emotion-types'],
    queryFn: getSupportedEmotionTypes,
  });
};

export const useEmotions = (humanId: string | undefined) => {
  return useQuery({
    queryKey: ['emotions', humanId],
    queryFn: () => getHumanEmotions(humanId!),
    enabled: !!humanId,
  });
};

export const useAddEmotion = (humanId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ type, percentage }: { type: EmotionType; percentage: number }) =>
      addEmotion(humanId, type, percentage),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['emotions', humanId] });
      queryClient.invalidateQueries({ queryKey: ['humans', humanId] });
      queryClient.invalidateQueries({ queryKey: ['humans'] });
    },
  });
};

export const useUpdateEmotion = (humanId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: ({ type, percentage }: { type: EmotionType; percentage: number }) =>
      updateEmotion(humanId, type, percentage),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['emotions', humanId] });
      queryClient.invalidateQueries({ queryKey: ['humans', humanId] });
      queryClient.invalidateQueries({ queryKey: ['humans'] });
    },
  });
};

export const useDeleteEmotion = (humanId: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (type: EmotionType) => deleteEmotion(humanId, type),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['emotions', humanId] });
      queryClient.invalidateQueries({ queryKey: ['humans', humanId] });
      queryClient.invalidateQueries({ queryKey: ['humans'] });
    },
  });
};

