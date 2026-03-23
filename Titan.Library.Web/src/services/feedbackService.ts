import { apiClient } from '@/lib/api';

export interface SubmitFeedbackRequest {
  category: string;
  rating: number | null;
  subject: string;
  message: string;
}

export interface FeedbackDto {
  id: number;
  customerId: number;
  category: string;
  rating: number | null;
  subject: string;
  message: string;
  createdAt: string;
}

export const feedbackService = {
  submit: (data: SubmitFeedbackRequest) =>
    apiClient.post<FeedbackDto>('/Feedbacks', data),
};
