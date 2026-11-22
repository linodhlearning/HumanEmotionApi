export type EmotionType = 1 | 2 | 3 | 4 | 5;

export const EMOTION_TYPES: EmotionType[] = [1, 2, 3, 4, 5];

export const EMOTION_LABELS: Record<EmotionType, string> = {
  1: 'Joy',
  2: 'Sadness',
  3: 'Anger',
  4: 'Fear',
  5: 'Disgust',
};

export const EMOTION_EMOJIS: Record<EmotionType, string> = {
  1: '😊',
  2: '😢',
  3: '😡',
  4: '😨',
  5: '🤢',
};

export const EMOTION_COLORS: Record<EmotionType, string> = {
  1: '#EAB308', // Joy - warm yellow/amber
  2: '#3B82F6', // Sadness - blue
  3: '#EF4444', // Anger - red
  4: '#8B5CF6', // Fear - purple
  5: '#10B981', // Disgust - green
};

export const NAME_TO_EMOTION_TYPE: Record<string, EmotionType> = {
  Joy: 1,
  Sadness: 2,
  Anger: 3,
  Fear: 4,
  Disgust: 5,
};

export interface EmotionDto {
  id: string;
  humanId: string;
  type: string; // "Joy" | "Sadness" etc.
  percentage: number;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}

export interface HumanDto {
  id: string;
  name: string;
  showColor: string; // e.g. "#FFFFFF" -> "#008000"
  emotionScorePercentage: number; // 0 - 100
  emotions: EmotionDto[];
  createdAtUtc: string;
  updatedAtUtc?: string | null;
}

export interface CreateHumanRequest {
  name: string;
}

export interface UpdateHumanRequest {
  name: string;
}

export interface AddEmotionRequest {
  type: EmotionType;
  percentage: number;
}

export interface UpdateEmotionRequest {
  percentage: number;
}

