import { Library, BookOpen, Users, Target, Heart, Star } from 'lucide-react';
import { PageHeader } from '@/components/layout/PageHeader';

// ─── Page ─────────────────────────────────────────────────────────────────────

export function AboutUsPage() {
  return (
    <div className="space-y-10">
      <PageHeader
        title="About Us"
        description="Learn more about Titan Library and our mission"
      />

      {/* ── Hero ─────────────────────────────────────────────────────────────── */}
      <div className="bg-gradient-to-br from-indigo-600 to-purple-700 rounded-2xl p-8 text-white shadow-lg">
        <div className="flex items-center gap-3 mb-4">
          <div className="p-2.5 bg-white/20 rounded-xl">
            <Library size={28} className="text-white" />
          </div>
          <h2 className="text-2xl font-bold">Titan Library</h2>
        </div>
        <p className="text-indigo-100 text-base leading-relaxed max-w-2xl">
          Your gateway to a world of knowledge. We believe that access to books and learning
          should be seamless, modern, and enjoyable for everyone.
        </p>
      </div>

      {/* ── Mission & Vision ─────────────────────────────────────────────────── */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-6 shadow-sm">
          <div className="p-2.5 bg-indigo-50 dark:bg-indigo-950/30 rounded-xl w-fit mb-4">
            <Target size={22} className="text-indigo-600" />
          </div>
          <h3 className="text-lg font-semibold text-gray-900 dark:text-zinc-100 mb-2">Our Mission</h3>
          <p className="text-gray-600 dark:text-zinc-400 text-sm leading-relaxed">
            To provide a seamless and modern library experience that connects readers with the books
            they love. We strive to make borrowing, discovering, and managing books as effortless
            as possible.
          </p>
        </div>

        <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-6 shadow-sm">
          <div className="p-2.5 bg-purple-50 dark:bg-purple-950/30 rounded-xl w-fit mb-4">
            <Star size={22} className="text-purple-600" />
          </div>
          <h3 className="text-lg font-semibold text-gray-900 dark:text-zinc-100 mb-2">Our Vision</h3>
          <p className="text-gray-600 dark:text-zinc-400 text-sm leading-relaxed">
            A world where every person has easy access to knowledge and literature. We envision
            Titan Library as the leading platform that bridges the gap between readers and
            quality content.
          </p>
        </div>
      </div>

      {/* ── What we offer ────────────────────────────────────────────────────── */}
      <div className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-6 shadow-sm">
        <h3 className="text-lg font-semibold text-gray-900 dark:text-zinc-100 mb-6">What We Offer</h3>
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-6">
          {[
            {
              icon: BookOpen,
              color: 'indigo',
              title: 'Vast Collection',
              description: 'Browse thousands of books across every genre, from classic literature to modern bestsellers.',
            },
            {
              icon: Users,
              color: 'amber',
              title: 'Community',
              description: 'Connect with authors and fellow readers. Discover books recommended by people who share your taste.',
            },
            {
              icon: Heart,
              color: 'rose',
              title: 'Easy Borrowing',
              description: 'Borrow and return books with just a few clicks. Track your reading history effortlessly.',
            },
          ].map(({ icon: Icon, color, title, description }) => (
            <div key={title} className="flex flex-col items-start gap-3">
              <div className={`p-2.5 bg-${color}-50 dark:bg-${color}-950/30 rounded-xl`}>
                <Icon size={20} className={`text-${color}-600`} />
              </div>
              <div>
                <p className="font-semibold text-gray-900 dark:text-zinc-100 text-sm mb-1">{title}</p>
                <p className="text-gray-500 dark:text-zinc-400 text-sm leading-relaxed">{description}</p>
              </div>
            </div>
          ))}
        </div>
      </div>

      {/* ── Stats ────────────────────────────────────────────────────────────── */}
      <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
        {[
          { value: '10,000+', label: 'Books' },
          { value: '5,000+', label: 'Active Readers' },
          { value: '500+', label: 'Authors' },
          { value: '99%', label: 'Satisfaction' },
        ].map(({ value, label }) => (
          <div
            key={label}
            className="bg-white dark:bg-zinc-900 border border-gray-200 dark:border-zinc-800 rounded-2xl p-5 text-center shadow-sm"
          >
            <p className="text-2xl font-bold text-indigo-600 dark:text-indigo-400">{value}</p>
            <p className="text-sm text-gray-500 dark:text-zinc-400 mt-1">{label}</p>
          </div>
        ))}
      </div>

    </div>
  );
}
