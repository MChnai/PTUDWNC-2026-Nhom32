import { useState, useEffect, useCallback, useMemo, useRef, type FormEvent } from 'react';
import {
  Search,
  SlidersHorizontal,
  ChevronLeft,
  ChevronRight,
  Sparkles,
  BookOpen,
  CheckCircle2,
  AlertCircle,
} from 'lucide-react';
import { Header } from './components/Header';
import { RecipeCard } from './components/RecipeCard';
import { RecipeDetailModal } from './components/RecipeDetailModal';
import { RecipeWizardModal } from './components/RecipeWizardModal';
import { ApiExplorer } from './components/ApiExplorer';
import { ArchitectureViewer } from './components/ArchitectureViewer';
import { SystemMonitor } from './components/SystemMonitor';
import { Dashboard } from './components/Dashboard';
import { apiClient } from './services/api-client';
import { useAuthStore } from './store/auth-store';
import { type Recipe, type Category, type ApplicationUser, type UserRole, RecipeDifficulty, RecipeStatus } from './types';

type ApiResult<T> = {
  isSuccess: boolean;
  value: T;
  errorCode?: string | null;
  errorMessage?: string | null;
};

type ApiPage<T> = {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  draftCount: number;
  publishedCount: number;
  archivedCount: number;
};

type ApiRecipe = {
  id: string;
  title: string;
  slug: string;
  summary?: string | null;
  coverImage?: string | null;
  prepTimeMinutes: number;
  cookTimeMinutes: number;
  servings?: number;
  difficulty: number;
  status?: number;
  categoryId: string;
  categoryName?: string;
  authorId?: string;
  authorName?: string;
  publishedAt?: string | null;
  createdAt?: string;
  viewCount?: number;
  averageRating?: number;
  ratingCount?: number;
  items: ApiRecipe[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  draftCount: number;
  publishedCount: number;
  archivedCount: number;
};

type ManagementApiResponse = {
  items: ApiRecipe[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  draftCount: number;
  publishedCount: number;
  archivedCount: number;
};

// Backend enum is 1/2/3. Convert at the API boundary so this file also works
// before the legacy frontend enum in types.ts is updated.
const serverRecipeStatus = { Draft: 1, Published: 2, Archived: 3 } as const;
const toUiRecipeStatus = (status: number): Recipe['status'] => {
  if (status === serverRecipeStatus.Draft) return RecipeStatus.Draft;
  if (status === serverRecipeStatus.Archived) return RecipeStatus.Archived;
  return RecipeStatus.Published;
};

const apiErrorMessage = (error: unknown): string => {
  if (error && typeof error === 'object') {
    const e = error as {
      message?: string;
      response?: { data?: { errorMessage?: string; message?: string; detail?: string } };
    };
    return e.response?.data?.errorMessage || e.response?.data?.message ||
      e.response?.data?.detail || e.message || 'Unknown API error';
  }
  return String(error);
};

// The list endpoint returns a summary, not a full Recipe. Full details are loaded on click.
const mapRecipe = (item: ApiRecipe, categories: Category[]): Recipe => ({
  id: item.id,
  title: item.title,
  slug: item.slug,
  description: item.summary ?? '',
  instructions: '',
  prepTime: item.prepTimeMinutes,
  cookTime: item.cookTimeMinutes,
  servings: item.servings ?? 0,
  difficulty: item.difficulty as Recipe['difficulty'],
  status: toUiRecipeStatus(item.status ?? serverRecipeStatus.Published),
  categoryId: item.categoryId,
  category: categories.find(category => category.id === item.categoryId),
  authorId: item.authorId ?? '',
  publishedAt: item.publishedAt ?? undefined,
  createdAt: item.createdAt ?? item.publishedAt ?? '',
  viewCount: item.viewCount ?? 0,
  ratingAverage: item.averageRating ?? 0,
  ratingCount: item.ratingCount ?? 0,
  isDeleted: false,
  nutrition: undefined,
  ingredients: [],
  steps: [],
  images: item.coverImage ? [{
    id: `cover-${item.id}`,
    recipeId: item.id,
    originalUrl: resolveImageUrl(item.coverImage),
    isPrimary: true,
    orderIndex: 0,
  }] : [],
});

const resolveImageUrl = (url: string): string => {
  const apiBase =
    import.meta.env.VITE_API_BASE_URL ||
    'http://localhost:5000/api/v1';

  return new URL(url, apiBase).toString();
};


async function syncIngredients(
  recipeId: string,
  oldItems: Recipe['ingredients'],
  newItems: Recipe['ingredients']
) {
  const oldIds = new Set(oldItems.map(i => i.id));

  const newIds = new Set(
    newItems.map(i => i.id).filter(Boolean)
  );

  // Xóa các nguyên liệu người dùng đã bỏ khỏi form
  for (const oldItem of oldItems) {
    if (!newIds.has(oldItem.id)) {
      await apiClient.delete(
        `/recipes/${recipeId}/ingredients/${oldItem.id}`
      );
    }
  }

  // Thêm mới hoặc cập nhật
  for (const [index, item] of newItems.entries()) {
    const payload = {
      name: item.name,
      quantity: item.quantity ?? 1,
      unit: item.unit ?? '',
      notes: item.notes ?? '',
      sortOrder: index + 1
    };

    if (item.id && oldIds.has(item.id)) {
      await apiClient.put(
        `/recipes/${recipeId}/ingredients/${item.id}`,
        payload
      );
    } else {
      await apiClient.post(
        `/recipes/${recipeId}/ingredients`,
        payload
      );
    }
  }
}
async function syncSteps(
  recipeId: string,
  oldSteps: Recipe['steps'],
  newSteps: Recipe['steps']
) {
  const oldIds = new Set(oldSteps.map(s => s.id));

  const newIds = new Set(
    newSteps.map(s => s.id).filter(Boolean)
  );

  // Xóa bước không còn tồn tại
  for (const oldStep of oldSteps) {
    if (!newIds.has(oldStep.id)) {
      await apiClient.delete(
        `/recipes/${recipeId}/steps/${oldStep.id}`
      );
    }
  }

  // Thêm mới hoặc cập nhật
  for (const [index, step] of newSteps.entries()) {
    const payload = {
      title: step.title?.trim() || `Bước ${index + 1}`,
      description: step.description ?? '',
      timerMinutes: step.timerMinutes ?? null,
      imageUrl: step.imageUrl ?? null
    };

    if (step.id && oldIds.has(step.id)) {
      await apiClient.put(
        `/recipes/${recipeId}/steps/${step.id}`,
        payload
      );
    } else {
      await apiClient.post(
        `/recipes/${recipeId}/steps`,
        payload
      );
    }
  }
}

// These small API-backed dialogs avoid the synchronous mock callbacks still used by
// the original AuthModal, CategoryManagerModal and SearchModal components.
function CategoryApiModal({
  categories, onClose, onRefresh,
}: {
  categories: Category[];
  onClose: () => void;
  onRefresh: () => Promise<void>;
}) {
  const [editingId, setEditingId] = useState<string | null>(null);
  const [name, setName] = useState('');
  const [description, setDescription] = useState('');
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState('');

  const save = async (event: FormEvent) => {
    event.preventDefault();
    if (!name.trim()) return;
    setBusy(true);
    setMessage('');
    try {
      if (editingId) {
        await apiClient.put(`/categories/${editingId}`, { name: name.trim(), description });
      } else {
        await apiClient.post('/categories', { name: name.trim(), description });
      }
      await onRefresh();
      setMessage('Đã lưu danh mục vào server.');
      setEditingId(null);
      setName('');
      setDescription('');
    } catch (error) {
      setMessage(`Không thể lưu: ${apiErrorMessage(error)}`);
    } finally {
      setBusy(false);
    }
  };
  const remove = async (category: Category) => {
    if (!window.confirm(`Xóa danh mục '${category.name}'?`)) return;
    setBusy(true);
    setMessage('');
    try {
      await apiClient.delete(`/categories/${category.id}`);
      await onRefresh();
      setMessage('Đã xóa danh mục.');
    } catch (error) {
      setMessage(`Không thể xóa: ${apiErrorMessage(error)}`);
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 bg-stone-900/60 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-2xl max-h-[90vh] overflow-y-auto p-6 space-y-5">
        <div className="flex items-center justify-between">
          <h2 className="text-xl font-bold">Manage Categories (API)</h2>
          <button onClick={onClose} className="text-stone-600 hover:text-stone-900">Close</button>
        </div>
        <form onSubmit={save} className="space-y-3 border rounded-xl p-4 bg-stone-50">
          <input required value={name} onChange={event => setName(event.target.value)}
            placeholder="Category name" className="w-full border rounded-lg p-2" />
          <textarea value={description} onChange={event => setDescription(event.target.value)}
            placeholder="Description" className="w-full border rounded-lg p-2" />
          <div className="flex gap-2">
            <button disabled={busy} type="submit" className="px-4 py-2 rounded-lg bg-amber-600 text-white disabled:opacity-50">
              {editingId ? 'Update' : 'Create'}
            </button>
            {editingId && <button type="button" onClick={() => { setEditingId(null); setName(''); setDescription(''); }}
              className="px-4 py-2 rounded-lg bg-stone-200">Cancel edit</button>}
          </div>
        </form>
        {message && <p role="status" className="text-sm text-stone-700">{message}</p>}
        <div className="divide-y border rounded-xl">
          {categories.map(category => (
            <div key={category.id} className="flex items-center justify-between gap-3 px-4 py-3">
              <div><p className="font-semibold">{category.name}</p><p className="text-xs text-stone-500">{category.recipeCount ?? 0} recipes</p></div>
              <div className="flex gap-3 text-sm">
                <button disabled={busy} className="text-amber-700" onClick={() => {
                  setEditingId(category.id); setName(category.name); setDescription(category.description ?? '');
                }}>Edit</button>
                <button disabled={busy} className="text-rose-700" onClick={() => void remove(category)}>Delete</button>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}

function SearchApiModal({ categories, onClose, onSelect }: {
  categories: Category[];
  onClose: () => void;
  onSelect: (recipe: Recipe) => void;
}) {
  const [keyword, setKeyword] = useState('');
  const [categoryId, setCategoryId] = useState('');
  const [results, setResults] = useState<Recipe[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    let cancelled = false;
    if (!keyword.trim()) { setResults([]); setLoading(false); setError(''); return; }
    setLoading(true);
    const timer = window.setTimeout(async () => {
      try {
        const response = await apiClient.get<ApiResult<ApiPage<ApiRecipe>>>('/recipes/search', {
          params: { keyword: keyword.trim(), categoryId: categoryId || undefined, page: 1, pageSize: 20 },
        });
        if (!response.data.isSuccess) throw new Error(response.data.errorMessage || 'Search failed');
        if (!cancelled) { setResults(response.data.value.items.map(item => mapRecipe(item, categories))); setError(''); }
      } catch (e) {
        if (!cancelled) { setError(apiErrorMessage(e)); setResults([]); }
      } finally {
        if (!cancelled) setLoading(false);
      }
    }, 300);
    return () => { cancelled = true; window.clearTimeout(timer); };
  }, [keyword, categoryId, categories]);

  return (
    <div className="fixed inset-0 z-50 bg-stone-900/60 flex items-start justify-center p-4 pt-20">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-2xl p-5 max-h-[80vh] overflow-y-auto space-y-4">
        <div className="flex gap-2">
          <input autoFocus value={keyword} onChange={e => setKeyword(e.target.value)} placeholder="Search recipes (PostgreSQL FTS)"
            className="flex-1 border rounded-lg p-2" />
          <button onClick={onClose} className="px-3 rounded-lg bg-stone-100">Close</button>
        </div>
        <select value={categoryId} onChange={e => setCategoryId(e.target.value)} className="border rounded-lg p-2 w-full">
          <option value="">All categories</option>
          {categories.map(category => <option key={category.id} value={category.id}>{category.name}</option>)}
        </select>
        {loading && <p className="text-sm text-stone-500">Searching...</p>}
        {error && <p className="text-sm text-rose-700">{error}</p>}
        {!loading && keyword.trim() && !error && results.length === 0 && <p className="text-sm text-stone-500">No matching recipes.</p>}
        <div className="divide-y">
          {results.map(recipe => <button key={recipe.id} className="w-full text-left py-3 hover:bg-stone-50"
            onClick={() => { onSelect(recipe); onClose(); }}>
            <span className="font-semibold">{recipe.title}</span>
            <p className="text-sm text-stone-500 line-clamp-1">{recipe.description}</p>
          </button>)}
        </div>
      </div>
    </div>
  );
}

type LoginData = {
  userId: string;
  username: string;
  email: string;
  role: string;
  accessToken: string;
  refreshToken: string;
};

function AuthApiModal({ initialTab, currentUser, onClose, onAuthenticated, onLogout }: {
  initialTab: 'login' | 'register' | 'tokens';
  currentUser: ApplicationUser | null;
  onClose: () => void;
  onAuthenticated: () => void;
  onLogout: () => void;
}) {
  const [tab, setTab] = useState<'login' | 'register'>(initialTab === 'register' ? 'register' : 'login');
  const [email, setEmail] = useState('');
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState('');

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    setBusy(true);
    setMessage('');
    try {
      if (tab === 'register') {
        await apiClient.post('/auth/register', { username, email, password });
        setMessage('Đăng ký thành công. Hãy đăng nhập để nhận JWT.');
        setTab('login');
      } else {
        const response = await apiClient.post<ApiResult<LoginData>>('/auth/login', { email, password });
        if (!response.data.isSuccess || !response.data.value) {
          throw new Error(response.data.errorMessage || 'Đăng nhập thất bại');
        }
        const value = response.data.value;
        useAuthStore.getState().setCredentials({
          id: value.userId,
          email: value.email,
          userName: value.username,
          fullName: value.username,
          roles: [value.role],
        }, value.accessToken, value.refreshToken);
        onAuthenticated();
        onClose();
      }
    } catch (error) {
      setMessage(apiErrorMessage(error));
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 bg-stone-900/60 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl shadow-xl w-full max-w-md p-6 space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-xl font-bold">{currentUser ? 'Your Account' : 'Authentication (API)'}</h2>
          <button onClick={onClose} className="text-stone-500">Close</button>
        </div>
        {currentUser ? (
          <div className="space-y-3">
            <p>{currentUser.email} ({currentUser.roles.join(', ')})</p>
            <button onClick={() => { onLogout(); onClose(); }} className="rounded-lg bg-stone-900 text-white px-4 py-2">Sign out</button>
          </div>
        ) : (
          <>
            <div className="flex gap-3 text-sm">
              <button onClick={() => setTab('login')} className={tab === 'login' ? 'font-bold text-amber-700' : ''}>Login</button>
              <button onClick={() => setTab('register')} className={tab === 'register' ? 'font-bold text-amber-700' : ''}>Register</button>
            </div>
            <form onSubmit={submit} className="space-y-3">
              {tab === 'register' && <input required value={username} onChange={e => setUsername(e.target.value)} placeholder="Username"
                className="w-full p-2 border rounded-lg" />}
              <input required type="email" value={email} onChange={e => setEmail(e.target.value)} placeholder="Email"
                className="w-full p-2 border rounded-lg" />
              <input required type="password" value={password} onChange={e => setPassword(e.target.value)} placeholder="Password"
                className="w-full p-2 border rounded-lg" />
              <button type="submit" disabled={busy} className="w-full rounded-lg bg-amber-600 text-white p-2 disabled:opacity-50">
                {busy ? 'Please wait...' : tab === 'login' ? 'Login' : 'Create account'}
              </button>
            </form>
            {message && <p role="status" className="text-sm text-stone-700">{message}</p>}
          </>
        )}
      </div>
    </div>
  );
}

export default function App() {
  const [activeTab, setActiveTab] = useState<'explore' | 'dashboard' | 'api' | 'architecture' | 'monitor'>('explore');
  const authUser = useAuthStore(state => state.user);
  const logout = useAuthStore(state => state.logout);
  const currentUser = useMemo<ApplicationUser | null>(() => authUser ? ({
    id: authUser.id,
    email: authUser.email,
    userName: authUser.userName,
    displayName: authUser.fullName || authUser.userName,
    avatarUrl: authUser.avatarUrl,
    roles: authUser.roles.filter((role): role is UserRole => role === 'Admin' || role === 'Author'),
    isActive: true,
    emailConfirmed: false,
    createdAt: '',
  }) : null, [authUser]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [recipes, setRecipes] = useState<Recipe[]>([]);
  // Dashboard Management
  const [managementRecipes, setManagementRecipes] = useState<Recipe[]>([]);
  const [managementPage, setManagementPage] = useState<number>(1);
  const [managementTotalPages, setManagementTotalPages] = useState<number>(1);
  const [managementStatus, setManagementStatus] = useState<string>('all');
  const [managementCategory, setManagementCategory] = useState<string>('all');
  const [managementCounts, setManagementCounts] = useState({
    draftCount: 0,
    publishedCount: 0,
    archivedCount: 0,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [loadError, setLoadError] = useState<string | null>(null);
  const requestSequence = useRef(0);

  // Explore filtering & pagination states
  const [selectedCategory, setSelectedCategory] = useState<string>('all');
  const [selectedDifficulty, setSelectedDifficulty] = useState<string>('all');
  const [sortField, setSortField] = useState<string>('-createdAt');
  const [maxCookTime, setMaxCookTime] = useState<number>(240);
  const [currentPage, setCurrentPage] = useState<number>(1);
  const pageSize = 6;
  const [totalCount, setTotalCount] = useState<number>(0);
  const [totalPages, setTotalPages] = useState<number>(1);

  // Modals state
  const [selectedRecipe, setSelectedRecipe] = useState<Recipe | null>(null);
  const [recipeToEdit, setRecipeToEdit] = useState<Recipe | null>(null);
  const [isWizardOpen, setIsWizardOpen] = useState(false);
  const [isCategoryManagerOpen, setIsCategoryManagerOpen] = useState(false);
  const [isSearchOpen, setIsSearchOpen] = useState(false);
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [authModalTab, setAuthModalTab] = useState<'login' | 'register' | 'tokens'>('login');

  // Toast notification
  const [toast, setToast] = useState<{ message: string; type: 'success' | 'error' | 'info' } | null>(null);

  const showToast = (message: string, type: 'success' | 'error' | 'info' = 'success') => {
    setToast({ message, type });
    setTimeout(() => setToast(null), 3500);
  };

  const loadCategories = useCallback(async () => {
    const response = await apiClient.get<
      ApiResult<Array<Pick<Category, 'id' | 'name' | 'slug' | 'description' | 'recipeCount'>>>
    >('/categories');

    if (!response.data.isSuccess) {
      throw new Error(
        response.data.errorMessage || 'Failed to load categories'
      );
    }

    setCategories(response.data.value.map((category, index) => ({
      ...category,
      orderIndex: index,
      createdAt: '',
      isDeleted: false,
    })));
  }, []);


  const loadData = useCallback(async () => {
    const sequence = ++requestSequence.current;
    setIsLoading(true);
    setLoadError(null);
    try {
      const descending = sortField.startsWith('-');
      const sortBy = sortField.replace(/^-/, '');
      const response = await apiClient.get<ApiResult<ApiPage<ApiRecipe>>>('/recipes', {
        params: {
          pageNumber: currentPage,
          pageSize,
          categoryId: selectedCategory !== 'all' ? selectedCategory : undefined,
          difficulty: selectedDifficulty !== 'all' ? Number(selectedDifficulty) : undefined,
          maxTotalTime: maxCookTime < 240 ? maxCookTime : undefined,
          sortBy,
          sortDirection: descending ? 'desc' : 'asc',
        },
      });
      if (!response.data.isSuccess || !response.data.value) {
        throw new Error(response.data.errorMessage || 'Failed to load recipes');
      }
      if (sequence !== requestSequence.current) return;
      setRecipes(response.data.value.items.map(recipe => mapRecipe(recipe, categories)));
      setTotalCount(response.data.value.totalCount);
      setTotalPages(Math.max(1, response.data.value.totalPages));
    } catch (error) {
      if (sequence !== requestSequence.current) return;
      setLoadError(apiErrorMessage(error));
      setRecipes([]);
      setTotalCount(0);
      setTotalPages(1);
    } finally {
      if (sequence === requestSequence.current) setIsLoading(false);
    }
  }, [currentPage, pageSize, selectedCategory, selectedDifficulty, maxCookTime, sortField, categories]);
  const loadManagementRecipes = useCallback(async () => {
    const response = await apiClient.get<
      ApiResult<ManagementApiResponse>
    >('/recipes/manage', {
      params: {
        pageNumber: managementPage,
        pageSize: 20,
        status: managementStatus !== 'all'
          ? Number(managementStatus)
          : undefined,
        categoryId: managementCategory !== 'all'
          ? managementCategory
          : undefined,
      },
    });

    if (!response.data.isSuccess || !response.data.value) {
      throw new Error(
        response.data.errorMessage ||
        'Không thể tải dữ liệu quản trị'
      );
    }

    const value = response.data.value;

    setManagementRecipes(
      value.items.map(item => mapRecipe(item, categories))
    );

    setManagementTotalPages(
      Math.max(1, value.totalPages)
    );

    setManagementCounts({
      draftCount: value.draftCount,
      publishedCount: value.publishedCount,
      archivedCount: value.archivedCount,
    });
  }, [
    managementPage,
    managementStatus,
    managementCategory,
    categories,
  ]);
  useEffect(() => {
    const canManage = currentUser?.roles.some(
      role => role === 'Admin' || role === 'Author'
    );

    if (activeTab !== 'dashboard' || !canManage) {
      setManagementRecipes([]);
      return;
    }

    void loadManagementRecipes().catch(error => {
      showToast(
        `Lỗi tải Dashboard: ${apiErrorMessage(error)}`,
        'error'
      );
    });
  }, [
    activeTab,
    currentUser?.id,
    loadManagementRecipes,
  ]);
  useEffect(() => {
    void loadCategories().catch(error => {
      showToast(`Không thể tải danh mục: ${apiErrorMessage(error)}`, 'error');
    });
  }, [loadCategories]);

  useEffect(() => {
    void loadData();
    return () => { requestSequence.current += 1; };
  }, [loadData]);

  useEffect(() => {
    setCurrentPage(1);
  }, [selectedCategory, selectedDifficulty, maxCookTime, sortField]);

  // Keyboard shortcut cmd+k / ctrl+k for search
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if ((e.metaKey || e.ctrlKey) && e.key === 'k') {
        e.preventDefault();
        setIsSearchOpen(true);
      }
    };
    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, []);

  const handleSwitchRole = (role: UserRole) => {
    if (role === 'Guest') {
      logout();
      showToast('Đã đăng xuất', 'info');
      return;
    }
    setAuthModalTab('login');
    setIsAuthModalOpen(true);
    showToast(`Đăng nhập tài khoản có quyền ${role}; không thể giả lập vai trò.`, 'info');
  };

  const handleSelectRecipe = async (recipe: Recipe) => {
    try {
      // GET /recipes/{id} must be implemented in RecipeEndpoints.cs.
      const response = await apiClient.get(`/recipes/${recipe.id}`);
      const detail = response.data?.value ?? response.data;
      if (!detail?.id) throw new Error('Invalid recipe detail response');
      const fullRecipe: Recipe = {
        ...recipe,
        ...detail,
        description: detail.description ?? recipe.description,
        status: toUiRecipeStatus(detail.status ?? serverRecipeStatus.Published),
        prepTime: detail.prepTime ?? detail.prepTimeMinutes ?? recipe.prepTime,
        cookTime: detail.cookTime ?? detail.cookTimeMinutes ?? recipe.cookTime,
        category: categories.find(category => category.id === detail.categoryId) ?? recipe.category,
        rowVersion: detail.rowVersion == null ? undefined : String(detail.rowVersion),
        ingredients: detail.ingredients ?? [],
        steps: (detail.steps ?? []).map((step: Recipe['steps'][number] & { instruction?: string }) => ({
          ...step,
          title: step.title || `Bước ${step.stepNumber}`,
          description: step.description ?? step.instruction ?? '',
        })),
        images: (detail.images ?? []).map((image: Recipe['images'][number] & { displayOrder?: number }) => ({
          ...image,
          originalUrl: resolveImageUrl(image.originalUrl),
          orderIndex: image.orderIndex ?? image.displayOrder ?? 0,
        })),
      };
      setSelectedRecipe(fullRecipe);
      return fullRecipe;
    } catch (error) {
      showToast(`Không thể tải chi tiết món ăn: ${apiErrorMessage(error)}`, 'error');
    }
  };

  const uploadRecipeImage = async (
    recipeId: string,
    file: File
  ) => {
    const formData = new FormData();

    formData.append('file', file);
    formData.append('altText', file.name);
    formData.append('isPrimary', 'true');

    await apiClient.post(
      `/recipes/${recipeId}/images`,
      formData,
      {
        headers: {
          'Content-Type': undefined,
        },
      }
    );
  };

  const handleSaveRecipe = async (data: Partial<Recipe>, publishImmediately = false, imageFile: File | null = null) => {
    if (!currentUser) {
      showToast('Vui lòng đăng nhập trước khi lưu công thức.', 'error');
      return;
    }

    const commonPayload = {
      title: data.title?.trim() ?? '',
      description: data.description ?? '',
      prepTimeMinutes: data.prepTime ?? 0,
      cookTimeMinutes: data.cookTime ?? 0,
      servings: data.servings ?? 4,
      difficulty: data.difficulty ?? RecipeDifficulty.Easy,
      categoryId: data.categoryId,
    };

    try {
      if (recipeToEdit) {
        if (!recipeToEdit.rowVersion) {
          throw new Error('Thiếu RowVersion. Hãy mở công thức từ API chi tiết trước khi sửa.');
        }
        await apiClient.put(`/recipes/${recipeToEdit.id}`, commonPayload, {
          headers: { 'If-Match': `"${recipeToEdit.rowVersion}"` },
        });
                if (imageFile) {
          await uploadRecipeImage(
            recipeToEdit.id,
            imageFile
          );
        }
        await syncIngredients(
          recipeToEdit.id,
          recipeToEdit.ingredients,
          data.ingredients ?? []
        );

        await syncSteps(
          recipeToEdit.id,
          recipeToEdit.steps,
          data.steps ?? []
        );
        if (publishImmediately && recipeToEdit.status !== RecipeStatus.Published) {
          await apiClient.patch(`/recipes/${recipeToEdit.id}/status`, {
            status: serverRecipeStatus.Published,
          });
        }
        showToast('Đã lưu thông tin chính. Nguyên liệu, bước và ảnh cần API cập nhật riêng.', 'info');
      } else {
        const payload = {
          ...commonPayload,
          authorId: currentUser.id, // The API replaces this with the JWT identity.
          instructions: '',
          notes: '',
          nutrition: data.nutrition
            ? {
              calories: data.nutrition.calories ?? 0,
              protein: data.nutrition.protein ?? 0,
              carbohydrates: data.nutrition.carbohydrates ?? 0,
              fat: data.nutrition.fat ?? 0,
              fiber: data.nutrition.fiber ?? null,
              sugar: null,
              sodium: data.nutrition.sodium ?? null
            }
            : null,
          ingredients: (data.ingredients ?? []).map((ingredient, index) => ({
            name: ingredient.name,
            quantity: ingredient.quantity ?? 1,
            unit: ingredient.unit ?? '',
            notes: ingredient.notes ?? '',
            orderIndex: index + 1,
          })),
          steps: (data.steps ?? []).map((step, index) => ({
            stepNumber: index + 1,
            title: step.title?.trim() || `Bước ${index + 1}`,
            description: step.description ?? '',
            timerMinutes: step.timerMinutes ?? null,
            imageUrl: step.imageUrl ?? null,
          })),
        };
        const response = await apiClient.post<ApiResult<string>>('/recipes', payload);
        if (!response.data.isSuccess || !response.data.value) {
          throw new Error(response.data.errorMessage || 'Failed to create recipe');
        }
        if (imageFile) {
          await uploadRecipeImage(
            response.data.value,
            imageFile
          );
        }
        if (publishImmediately) {
          await apiClient.patch(`/recipes/${response.data.value}/status`, {
            status: serverRecipeStatus.Published,
          });
        }
        showToast(publishImmediately
          ? 'Đã tạo và xuất bản công thức.'
          : 'Đã tạo bản nháp trong PostgreSQL.');
      }
      setIsWizardOpen(false);
      setRecipeToEdit(null);
      await loadData();
    } catch (error) {
      showToast(`Lưu công thức thất bại: ${apiErrorMessage(error)}`, 'error');
    }
  };

  const handlePublishToggle = async (recipe: Recipe) => {
    const willPublish = recipe.status !== RecipeStatus.Published;
    try {
      await apiClient.patch(`/recipes/${recipe.id}/status`, {
        status: willPublish ? serverRecipeStatus.Published : serverRecipeStatus.Draft,
      });
      setSelectedRecipe(null);
      await loadData();
      showToast(willPublish ? 'Đã xuất bản công thức.' : 'Đã chuyển về bản nháp.');
    } catch (error) {
      showToast(apiErrorMessage(error), 'error');
    }
  };

  const handleArchiveToggle = async (recipe: Recipe) => {
    try {
      await apiClient.patch(`/recipes/${recipe.id}/archive`, {
        isArchived: recipe.status !== RecipeStatus.Archived,
      });
      setSelectedRecipe(null);
      await loadData();
      showToast('Đã cập nhật trạng thái lưu trữ.');
    } catch (error) {
      showToast(apiErrorMessage(error), 'error');
    }
  };

  const handleDeleteRecipe = async (recipe: Recipe) => {
    if (!window.confirm(`Bạn chắc chắn muốn xóa '${recipe.title}'?`)) return;
    try {
      // The backend must map DeleteRecipeCommand as DELETE /recipes/{id}.
      await apiClient.delete(`/recipes/${recipe.id}`);
      setSelectedRecipe(null);
      await loadData();
      showToast('Đã xóa công thức trên server.');
    } catch (error) {
      showToast(apiErrorMessage(error), 'error');
    }
  };

  const handleResetSeed = () => {
    // Resetting a browser cache must never claim to reset the real database.
    void Promise.all([loadCategories(), loadData()])
      .then(() => showToast('Đã tải lại dữ liệu từ server; database không thay đổi.', 'info'))
      .catch(error => showToast(apiErrorMessage(error), 'error'));
  };

  return (
    <div className="min-h-screen bg-stone-50 text-stone-900 flex flex-col selection:bg-amber-100 selection:text-amber-900">
      {/* Toast Notification Bar */}
      {toast && (
        <div className="fixed bottom-5 right-5 z-50 animate-bounce">
          <div
            className={`px-4 py-3 rounded-xl shadow-lg border flex items-center gap-2.5 text-xs sm:text-sm font-medium ${toast.type === 'error'
              ? 'bg-rose-900 text-white border-rose-800'
              : toast.type === 'info'
                ? 'bg-sky-900 text-white border-sky-800'
                : 'bg-emerald-900 text-white border-emerald-800'
              }`}
          >
            {toast.type === 'error' ? (
              <AlertCircle className="w-4 h-4 text-rose-300 shrink-0" />
            ) : (
              <CheckCircle2 className="w-4 h-4 text-emerald-300 shrink-0" />
            )}
            <span>{toast.message}</span>
          </div>
        </div>
      )}

      {/* Main Global Header */}
      <Header
        activeTab={activeTab}
        setActiveTab={setActiveTab}
        currentUser={currentUser}
        onSwitchRole={handleSwitchRole}
        onOpenNewRecipe={() => {
          setRecipeToEdit(null);
          setIsWizardOpen(true);
        }}
        onOpenSearch={() => setIsSearchOpen(true)}
        searchTerm=""
        onOpenAuth={tab => {
          setAuthModalTab(tab || 'login');
          setIsAuthModalOpen(true);
        }}
      />

      {/* Main Body Content based on Active Tab */}
      <main className="flex-1 pb-16">
        {activeTab === 'explore' && (
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 space-y-8">
            {/* Hero Banner with system highlights */}
            <div className="relative rounded-3xl bg-gradient-to-r from-stone-900 via-amber-950 to-stone-900 text-white overflow-hidden p-8 sm:p-12 shadow-xl border border-stone-800">
              <div className="relative z-10 max-w-2xl space-y-4">
                <div className="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-amber-500/20 text-amber-300 text-xs font-semibold border border-amber-500/30">
                  <Sparkles className="w-3.5 h-3.5" />
                  <span>Culinary Blog & Recipe Platform V1.0</span>
                </div>

                <h1 className="text-3xl sm:text-5xl font-bold font-serif leading-tight">
                  Discover Culinary Art & Timeless Flavors
                </h1>

                <p className="text-sm sm:text-base text-stone-300 leading-relaxed">
                  Culinary recipe & product management platform powered by{' '}
                  <strong className="text-amber-300 font-semibold">.NET 10 Minimal APIs</strong>,{' '}
                  <strong className="text-amber-300 font-semibold">PostgreSQL 16</strong>, and{' '}
                  <strong className="text-amber-300 font-semibold">React + Vite</strong>.
                </p>

                <div className="pt-2 flex flex-wrap items-center gap-3">
                  <button
                    onClick={() => setIsSearchOpen(true)}
                    className="px-5 py-2.5 rounded-xl bg-amber-500 hover:bg-amber-400 text-stone-950 font-semibold text-xs sm:text-sm shadow-md transition-colors cursor-pointer flex items-center gap-2"
                  >
                    <Search className="w-4 h-4" />
                    <span>Search recipes (FTS)</span>
                  </button>

                  <button
                    onClick={() => setActiveTab('api')}
                    className="px-4 py-2.5 rounded-xl bg-white/10 hover:bg-white/15 text-white font-medium text-xs sm:text-sm border border-white/20 transition-colors cursor-pointer"
                  >
                    Explore REST API Spec (/scalar)
                  </button>
                </div>
              </div>

              {/* Ambient decoration */}
              <div className="absolute right-0 top-0 bottom-0 w-1/3 opacity-20 pointer-events-none hidden md:block">
                <img
                  src="https://images.unsplash.com/photo-1544025162-d76694265947?auto=format&fit=crop&w=800&q=80"
                  alt="Food artwork"
                  className="w-full h-full object-cover"
                />
              </div>
            </div>

            {/* Category Chips Carousel (FR-CAT-001) */}
            <div className="space-y-3">
              <div className="flex items-center justify-between">
                <h2 className="text-lg font-bold font-serif text-stone-900">
                  Categories ({categories.length})
                </h2>
                {currentUser?.roles.includes('Admin') && (
                  <button
                    onClick={() => setIsCategoryManagerOpen(true)}
                    className="text-xs text-amber-700 hover:text-amber-800 font-semibold cursor-pointer"
                  >
                    + Manage categories
                  </button>
                )}
              </div>

              <div className="flex items-center gap-2 overflow-x-auto pb-2 scrollbar-none">
                <button
                  onClick={() => {
                    setSelectedCategory('all');
                    setCurrentPage(1);
                  }}
                  className={`px-4 py-2 rounded-xl text-xs sm:text-sm font-semibold transition-all cursor-pointer shrink-0 ${selectedCategory === 'all'
                    ? 'bg-amber-600 text-white shadow-xs'
                    : 'bg-white text-stone-700 border border-stone-200 hover:bg-stone-50'
                    }`}
                >
                  All Recipes
                </button>

                {categories.map(cat => (
                  <button
                    key={cat.id}
                    onClick={() => {
                      setSelectedCategory(cat.id);
                      setCurrentPage(1);
                    }}
                    className={`px-4 py-2 rounded-xl text-xs sm:text-sm font-semibold transition-all cursor-pointer shrink-0 flex items-center gap-2 ${selectedCategory === cat.id
                      ? 'bg-amber-600 text-white shadow-xs'
                      : 'bg-white text-stone-700 border border-stone-200 hover:bg-stone-50'
                      }`}
                  >
                    <span>{cat.name}</span>
                    <span
                      className={`text-[11px] px-1.5 py-0.2 rounded-full ${selectedCategory === cat.id
                        ? 'bg-amber-700 text-white'
                        : 'bg-stone-100 text-stone-500'
                        }`}
                    >
                      {cat.recipeCount || 0}
                    </span>
                  </button>
                ))}
              </div>
            </div>

            {/* Filter & Sorting Bar (FR-RCP-001 & FR-SRCH-002/003) */}
            <div className="bg-white p-4 rounded-2xl border border-stone-200 shadow-xs flex flex-wrap items-center justify-between gap-4">
              <div className="flex flex-wrap items-center gap-3 text-xs">
                <div className="flex items-center gap-1.5 text-stone-500 font-medium">
                  <SlidersHorizontal className="w-3.5 h-3.5 text-stone-400" />
                  <span>Filters:</span>
                </div>

                {/* Difficulty Filter */}
                <select
                  value={selectedDifficulty}
                  onChange={e => {
                    setSelectedDifficulty(e.target.value);
                    setCurrentPage(1);
                  }}
                  className="px-3 py-1.5 bg-stone-50 border border-stone-200 rounded-lg text-xs font-medium text-stone-700 focus:ring-1 focus:ring-amber-500"
                >
                  <option value="all">Difficulty: All</option>
                  <option value={RecipeDifficulty.Easy}>Easy</option>
                  <option value={RecipeDifficulty.Medium}>Medium</option>
                  <option value={RecipeDifficulty.Hard}>Hard</option>
                  <option value={RecipeDifficulty.Expert}>Expert</option>
                </select>

                {/* The backend filters by prep + cook time, not cooking time alone. */}
                <div className="flex items-center gap-2 bg-stone-50 px-3 py-1 border border-stone-200 rounded-lg">
                  <span className="text-stone-500">Total Time ≤</span>
                  <input
                    type="range"
                    min="15"
                    max="240"
                    step="15"
                    value={maxCookTime}
                    onChange={e => {
                      setMaxCookTime(Number(e.target.value));
                      setCurrentPage(1);
                    }}
                    className="w-20 accent-amber-600"
                  />
                  <span className="font-mono font-semibold text-stone-800">
                    {maxCookTime >= 240 ? 'Any' : `${maxCookTime}m`}
                  </span>
                </div>
              </div>

              {/* Sorting Filter (FR-SRCH-003) */}
              <div className="flex items-center gap-2 text-xs">
                <span className="text-stone-500">Sort by:</span>
                <select
                  value={sortField}
                  onChange={e => setSortField(e.target.value)}
                  className="px-3 py-1.5 bg-stone-50 border border-stone-200 rounded-lg text-xs font-medium text-stone-700 focus:ring-1 focus:ring-amber-500"
                >
                  <option value="-createdAt">Newest first</option>
                  <option value="createdAt">Oldest first</option>
                  <option value="totalTime">Total time: Low to high</option>
                  <option value="-totalTime">Total time: High to low</option>
                  <option value="-views">Most viewed</option>
                  <option value="-rating">Highest rated</option>
                </select>
              </div>
            </div>

            {/* Recipes Grid */}
            <div className="space-y-4">
              <div className="flex items-center justify-between text-xs text-stone-500">
                <span>
                  Showing <strong className="text-stone-900">{recipes.length}</strong> of{' '}
                  <strong className="text-stone-900">{totalCount}</strong> recipes
                </span>
                <span className="font-mono text-[11px] text-stone-400">
                  OFFSET-LIMIT Pagination • Page {currentPage}/{totalPages}
                </span>
              </div>

              {loadError && (
                <div role="alert" className="bg-rose-50 border border-rose-200 rounded-xl p-4 text-sm text-rose-800">
                  Không thể kết nối dữ liệu thật: {loadError}
                  <button className="ml-3 underline" onClick={() => void loadData()}>Thử lại</button>
                </div>
              )}
              {isLoading ? (
                <div className="bg-white rounded-xl border p-8 text-center text-stone-500">Đang tải công thức từ backend...</div>
              ) : loadError ? null : recipes.length === 0 ? (
                <div className="bg-white rounded-2xl border border-stone-200 p-12 text-center space-y-3">
                  <BookOpen className="w-10 h-10 text-stone-300 mx-auto" />
                  <h3 className="font-bold text-stone-800 text-base">No recipes found</h3>
                  <p className="text-xs text-stone-500 max-w-sm mx-auto">
                    Try adjusting your category filter or total time threshold.
                  </p>
                  <button
                    onClick={() => {
                      setSelectedCategory('all');
                      setSelectedDifficulty('all');
                      setMaxCookTime(240);
                    }}
                    className="mt-2 px-4 py-1.5 rounded-lg bg-stone-100 hover:bg-stone-200 text-stone-700 text-xs font-semibold cursor-pointer"
                  >
                    Clear all filters
                  </button>
                </div>
              ) : (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                  {recipes.map(recipe => (
                    <RecipeCard
                      key={recipe.id}
                      recipe={recipe}
                      onSelect={r => { void handleSelectRecipe(r); }}
                    />
                  ))}
                </div>
              )}

              {/* Pagination Controls (FR-SRCH-004) */}
              {totalPages > 1 && (
                <div className="flex items-center justify-center gap-2 pt-6">
                  <button
                    disabled={currentPage <= 1}
                    onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                    className="p-2 rounded-lg border border-stone-200 bg-white hover:bg-stone-50 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                  >
                    <ChevronLeft className="w-4 h-4 text-stone-600" />
                  </button>

                  {Array.from({ length: totalPages }).map((_, i) => {
                    const pageNum = i + 1;
                    return (
                      <button
                        key={pageNum}
                        onClick={() => setCurrentPage(pageNum)}
                        className={`w-9 h-9 rounded-lg text-xs font-semibold transition-colors cursor-pointer ${currentPage === pageNum
                          ? 'bg-amber-600 text-white'
                          : 'bg-white border border-stone-200 text-stone-700 hover:bg-stone-50'
                          }`}
                      >
                        {pageNum}
                      </button>
                    );
                  })}

                  <button
                    disabled={currentPage >= totalPages}
                    onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                    className="p-2 rounded-lg border border-stone-200 bg-white hover:bg-stone-50 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer"
                  >
                    <ChevronRight className="w-4 h-4 text-stone-600" />
                  </button>
                </div>
              )}
            </div>
          </div>
        )}

        {activeTab === 'dashboard' && !currentUser && (
          <div className="max-w-7xl mx-auto mt-8 px-6 py-5 rounded-xl border border-amber-200 bg-amber-50 text-amber-900">
            Vui lòng đăng nhập bằng tài khoản Author hoặc Admin để quản lý công thức.
            <button className="ml-3 underline font-semibold" onClick={() => { setAuthModalTab('login'); setIsAuthModalOpen(true); }}>
              Đăng nhập
            </button>
          </div>
        )}
        {activeTab === 'dashboard' && currentUser && (
          <div className="max-w-7xl mx-auto mt-8 px-6 py-4 rounded-xl border border-amber-200 bg-amber-50 text-amber-900 text-sm">
            Backend hiện chỉ có API danh sách Published. Để Dashboard hiển thị cả Draft và Archived, cần thêm API
            <code className="mx-1">GET /api/v1/recipes/manage</code> có phân quyền Author/Admin.
          </div>
        )}
        {activeTab === 'dashboard' && currentUser && (
          <Dashboard
            currentUser={currentUser}
            categories={categories}
            recipes={managementRecipes}
            counts={managementCounts}
            filterStatus={managementStatus}
            filterCategory={managementCategory}

            onStatusChange={(value) => {
              setManagementPage(1);
              setManagementStatus(value);
            }}

            onCategoryChange={(value) => {
              setManagementPage(1);
              setManagementCategory(value);
            }}
            onSelectRecipe={r => { void handleSelectRecipe(r); }}
            onOpenNewRecipe={() => {
              setRecipeToEdit(null);
              setIsWizardOpen(true);
            }}
            onEditRecipe={r => {
              void handleSelectRecipe(r).then(detail => {
                if (!detail) return;
                setSelectedRecipe(null);
                setRecipeToEdit(detail);
                setIsWizardOpen(true);
              });
            }}
            onOpenCategoryManager={() => setIsCategoryManagerOpen(true)}
            onPublishToggle={handlePublishToggle}
            onArchiveToggle={handleArchiveToggle}
            onDeleteRecipe={handleDeleteRecipe}
            onResetSeed={handleResetSeed}
          />
        )}
        {activeTab === 'dashboard' && currentUser && (
          <div className="flex justify-center items-center gap-4 py-4">

            <button
              disabled={managementPage <= 1}
              onClick={() => {
                setManagementPage(prev => Math.max(1, prev - 1));
              }}
              className="px-4 py-2 border rounded-lg disabled:opacity-40"
            >
              Trang trước
            </button>

            <span className="text-sm font-medium">
              Trang {managementPage} / {managementTotalPages}
            </span>

            <button
              disabled={managementPage >= managementTotalPages}
              onClick={() => {
                setManagementPage(prev =>
                  Math.min(managementTotalPages, prev + 1)
                );
              }}
              className="px-4 py-2 border rounded-lg disabled:opacity-40"
            >
              Trang sau
            </button>

          </div>
        )}
        {activeTab === 'api' && (
          <div className="max-w-7xl mx-auto mt-6">
            <div role="status" className="mx-4 mb-3 rounded-xl p-3 bg-amber-50 border border-amber-200 text-amber-900 text-sm">
              API Explorer gốc vẫn là bản mô phỏng. Dùng trang Scalar của backend để thử API thật.
            </div>
            <ApiExplorer />
          </div>
        )}

        {activeTab === 'architecture' && <ArchitectureViewer />}

        {activeTab === 'monitor' && (
          <div className="max-w-7xl mx-auto mt-6">
            <div role="status" className="mx-4 mb-3 rounded-xl p-3 bg-amber-50 border border-amber-200 text-amber-900 text-sm">
              Các chỉ số trong System Monitor vẫn là dữ liệu mô phỏng cho đến khi có API giám sát thực tế.
            </div>
            <SystemMonitor />
          </div>
        )}
      </main>

      {/* Footer */}
      <footer className="bg-white border-t border-stone-200 py-8 text-xs text-stone-500">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 flex flex-col sm:flex-row items-center justify-between gap-4">
          <div>
            <p className="font-semibold text-stone-800">
              Culinary Blog – Advanced Web Application Engineering Platform
            </p>
            <p className="text-stone-400 mt-0.5">
              Compliant with IEEE 830 / ISO 29148:2018 • .NET 10 Minimal APIs & PostgreSQL 16
            </p>
          </div>

          <div className="flex items-center gap-3">
            <button
              onClick={() => setActiveTab('api')}
              className="text-stone-600 hover:text-stone-900 cursor-pointer"
            >
              Scalar OpenAPI
            </button>
            <span>•</span>
            <button
              onClick={() => setActiveTab('architecture')}
              className="text-stone-600 hover:text-stone-900 cursor-pointer"
            >
              Clean Architecture C#
            </button>
            <span>•</span>
            <button
              onClick={() => setActiveTab('monitor')}
              className="text-stone-600 hover:text-stone-900 cursor-pointer"
            >
              Health & Jobs
            </button>
          </div>
        </div>
      </footer>

      {/* Recipe Detail Modal */}
      {selectedRecipe && (
        <RecipeDetailModal
          recipe={selectedRecipe}
          currentUser={currentUser}
          onClose={() => setSelectedRecipe(null)}
          onEdit={r => {
            setSelectedRecipe(null);
            setRecipeToEdit(r);
            setIsWizardOpen(true);
          }}
          onPublishToggle={handlePublishToggle}
          onArchiveToggle={handleArchiveToggle}
          onDelete={handleDeleteRecipe}
        />
      )}

      {/* Recipe Wizard / Editor Modal */}
      {isWizardOpen && (
        <RecipeWizardModal
          recipeToEdit={recipeToEdit}
          categories={categories}
          onClose={() => {
            setIsWizardOpen(false);
            setRecipeToEdit(null);
          }}
          onSave={handleSaveRecipe}
        />
      )}

      {/* Category Manager Modal (Admin only) */}
      {isCategoryManagerOpen && (
        <CategoryApiModal
          categories={categories}
          onClose={() => setIsCategoryManagerOpen(false)}
          onRefresh={loadCategories}
        />
      )}

      {/* Search Modal */}
      {isSearchOpen && (
        <SearchApiModal
          categories={categories}
          onClose={() => setIsSearchOpen(false)}
          onSelect={r => { void handleSelectRecipe(r); }}
        />
      )}

      {isAuthModalOpen && (
        <AuthApiModal
          initialTab={authModalTab}
          currentUser={currentUser}
          onClose={() => setIsAuthModalOpen(false)}
          onAuthenticated={() => showToast('Đăng nhập backend thành công.', 'success')}
          onLogout={() => { logout(); showToast('Đã đăng xuất.', 'info'); }}
        />
      )}
    </div>
  );
}
