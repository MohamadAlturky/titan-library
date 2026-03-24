import { useState } from 'react';
import { MessageSquare, Send, CheckCircle, Star } from 'lucide-react';
import { toast } from 'sonner';
import { PageHeader } from '@/components/layout/PageHeader';
import { Button } from '@/components/ui/Button';
import { feedbackService } from '@/services/feedbackService';

// ─── Types ────────────────────────────────────────────────────────────────────

type FeedbackCategory = 'general' | 'bug' | 'feature' | 'book' | 'other';

interface FeedbackForm {
  category: FeedbackCategory;
  rating: number;
  subject: string;
  message: string;
}

const emptyForm: FeedbackForm = {
  category: 'general',
  rating: 0,
  subject: '',
  message: '',
};

const categories: { value: FeedbackCategory; label: string }[] = [
  { value: 'general', label: 'General' },
  { value: 'bug', label: 'Bug Report' },
  { value: 'feature', label: 'Feature Request' },
  { value: 'book', label: 'Book Suggestion' },
  { value: 'other', label: 'Other' },
];

// ─── Page ─────────────────────────────────────────────────────────────────────

export function SendFeedbackPage() {
  const [form, setForm] = useState<FeedbackForm>(emptyForm);
  const [submitted, setSubmitted] = useState(false);
  const [isSubmitting, setIsSubmitting] = useState(false);

  const isValid = form.subject.trim().length > 0 && form.message.trim().length > 0;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!isValid) return;

    setIsSubmitting(true);
    try {
      await feedbackService.submit({
        category: form.category,
        rating: form.rating > 0 ? form.rating : null,
        subject: form.subject.trim(),
        message: form.message.trim(),
      });
      setSubmitted(true);
      toast.success('Feedback submitted! Thank you.');
    } catch {
      // nothing to do
    } finally {
      setIsSubmitting(false);
    }
  };

  const handleReset = () => {
    setForm(emptyForm);
    setSubmitted(false);
  };

  // ── Success state ──────────────────────────────────────────────────────────
  if (submitted) {
    return (
      <div className="space-y-6">
        <PageHeader title="Send Feedback" description="We'd love to hear what you think" />
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-12 shadow-sm flex flex-col items-center gap-4 text-center">
          <div className="p-4 bg-green-100 dark:bg-green-950/30 rounded-full">
            <CheckCircle size={36} className="text-green-600" />
          </div>
          <h3 className="text-xl font-semibold text-gray-900 dark:text-zinc-100">Thank you for your feedback!</h3>
          <p className="text-gray-500 dark:text-zinc-400 text-sm max-w-sm">
            Your message has been received. We read every submission and use your input to
            improve Titan Library.
          </p>
          <Button onClick={handleReset} variant="secondary" className="mt-2 rounded-xl px-6">
            Send another
          </Button>
        </div>
      </div>
    );
  }

  // ── Form ──────────────────────────────────────────────────────────────────
  return (
    <div className="space-y-6">
      <PageHeader title="Send Feedback" description="We'd love to hear what you think" />

      <form onSubmit={handleSubmit} className="space-y-6">

        {/* ── Category ───────────────────────────────────────────────────────── */}
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-6 shadow-sm space-y-4">
          <div className="flex items-center gap-2 mb-1">
            <MessageSquare size={18} className="text-indigo-500" />
            <h3 className="font-semibold text-gray-900 dark:text-zinc-100">Feedback details</h3>
          </div>

          {/* Category */}
          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-zinc-400">
              Category
            </label>
            <div className="flex flex-wrap gap-2">
              {categories.map(cat => (
                <button
                  key={cat.value}
                  type="button"
                  onClick={() => setForm(f => ({ ...f, category: cat.value }))}
                  className={`px-4 py-1.5 rounded-full text-sm font-medium border transition-all duration-150 ${form.category === cat.value
                      ? 'bg-indigo-600 text-white border-indigo-600 shadow-sm shadow-indigo-500/30'
                      : 'bg-transparent text-gray-600 dark:text-zinc-400 border-gray-300 dark:border-zinc-700 hover:border-indigo-400 dark:hover:border-indigo-500'
                    }`}
                >
                  {cat.label}
                </button>
              ))}
            </div>
          </div>

          {/* Rating */}
          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-zinc-400">
              Overall Rating (optional)
            </label>
            <div className="flex items-center gap-1">
              {[1, 2, 3, 4, 5].map(star => (
                <button
                  key={star}
                  type="button"
                  onClick={() => setForm(f => ({ ...f, rating: f.rating === star ? 0 : star }))}
                  className="transition-transform hover:scale-110"
                  aria-label={`Rate ${star}`}
                >
                  <Star
                    size={24}
                    className={star <= form.rating
                      ? 'text-amber-400 fill-amber-400'
                      : 'text-gray-300 dark:text-zinc-600'}
                  />
                </button>
              ))}
              {form.rating > 0 && (
                <span className="ml-2 text-sm text-gray-500 dark:text-zinc-400">
                  {['', 'Poor', 'Fair', 'Good', 'Great', 'Excellent'][form.rating]}
                </span>
              )}
            </div>
          </div>

          {/* Subject */}
          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-zinc-400">
              Subject <span className="text-red-400">*</span>
            </label>
            <input
              type="text"
              value={form.subject}
              onChange={e => setForm(f => ({ ...f, subject: e.target.value }))}
              placeholder="Brief subject of your feedback..."
              maxLength={120}
              className="w-full px-4 py-2.5 bg-gray-50 dark:bg-zinc-800 border border-gray-200 dark:border-zinc-700 rounded-xl text-sm text-gray-900 dark:text-zinc-100 placeholder-gray-400 dark:placeholder-zinc-500 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all"
            />
          </div>

          {/* Message */}
          <div className="space-y-2">
            <label className="text-xs font-semibold uppercase tracking-wider text-gray-500 dark:text-zinc-400">
              Message <span className="text-red-400">*</span>
            </label>
            <textarea
              value={form.message}
              onChange={e => setForm(f => ({ ...f, message: e.target.value }))}
              placeholder="Tell us more about your experience, suggestion, or issue..."
              rows={5}
              maxLength={1000}
              className="w-full px-4 py-2.5 bg-gray-50 dark:bg-zinc-800 border border-gray-200 dark:border-zinc-700 rounded-xl text-sm text-gray-900 dark:text-zinc-100 placeholder-gray-400 dark:placeholder-zinc-500 focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-500 outline-none transition-all resize-none"
            />
            <p className="text-xs text-gray-400 dark:text-zinc-500 text-right">
              {form.message.length} / 1000
            </p>
          </div>
        </div>

        {/* ── Submit ─────────────────────────────────────────────────────────── */}
        <div className="flex justify-end">
          <Button
            type="submit"
            disabled={!isValid || isSubmitting}
            className="px-8 rounded-xl shadow-md shadow-indigo-500/20 gap-2"
          >
            <Send size={16} />
            {isSubmitting ? 'Sending…' : 'Send Feedback'}
          </Button>
        </div>

      </form>
    </div>
  );
}
