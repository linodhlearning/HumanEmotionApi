import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getHumans, getHumanById, createHuman, updateHuman, deleteHuman } from '../api/humans';
import type { CreateHumanRequest, UpdateHumanRequest } from '../types';

export const useHumans = () => {
  return useQuery({
    queryKey: ['humans'],
    queryFn: getHumans,
  });
};

export const useHuman = (id: string | undefined) => {
  return useQuery({
    queryKey: ['humans', id],
    queryFn: () => getHumanById(id!),
    enabled: !!id,
  });
};

export const useCreateHuman = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: CreateHumanRequest) => createHuman(request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['humans'] });
    },
  });
};

export const useUpdateHuman = (id: string) => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (request: UpdateHumanRequest) => updateHuman(id, request),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['humans'] });
      queryClient.invalidateQueries({ queryKey: ['humans', id] });
    },
  });
};

export const useDeleteHuman = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (id: string) => deleteHuman(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['humans'] });
    },
  });
};
