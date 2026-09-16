const API_BASE = process.env.REACT_APP_API_URL || 'https://society-saas-api.azurewebsites.net';

async function apiRequest<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('token') || localStorage.getItem('adminToken');
  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...(options.headers as Record<string, string> || {}),
  };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  const response = await fetch(`${API_BASE}${path}`, { ...options, headers });

  if (!response.ok) {
    const error = await response.json().catch(() => ({ message: 'Request failed' }));
    throw new Error(error.message || `HTTP ${response.status}`);
  }

  return response.json();
}

export const authApi = {
  login: (email: string, password: string) =>
    apiRequest<{ token: string; email: string; firstName: string; lastName: string; isSuperAdmin: boolean; tenantId: string | null; tenantName: string | null }>(
      '/api/v1/auth/login', { method: 'POST', body: JSON.stringify({ email, password }) }),

  register: (data: { email: string; password: string; firstName: string; lastName: string; mobile?: string }) =>
    apiRequest<any>('/api/v1/auth/register', { method: 'POST', body: JSON.stringify(data) }),

  me: () => apiRequest<any>('/api/v1/auth/me'),
};

export const dashboardApi = {
  get: () => apiRequest<any>('/api/v1/dashboard'),
};

export const flatsApi = {
  list: (page = 1, pageSize = 50, search = '') =>
    apiRequest<any>(`/api/v1/flats?page=${page}&pageSize=${pageSize}&search=${search}`),
  get: (id: string) => apiRequest<any>(`/api/v1/flats/${id}`),
  create: (data: any) => apiRequest<any>('/api/v1/flats', { method: 'POST', body: JSON.stringify(data) }),
  update: (id: string, data: any) => apiRequest<any>(`/api/v1/flats/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id: string) => apiRequest<any>(`/api/v1/flats/${id}`, { method: 'DELETE' }),
};

export const membersApi = {
  list: (page = 1, pageSize = 50, search = '') =>
    apiRequest<any>(`/api/v1/members?page=${page}&pageSize=${pageSize}&search=${search}`),
  get: (id: string) => apiRequest<any>(`/api/v1/members/${id}`),
  create: (data: any) => apiRequest<any>('/api/v1/members', { method: 'POST', body: JSON.stringify(data) }),
  update: (id: string, data: any) => apiRequest<any>(`/api/v1/members/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id: string) => apiRequest<any>(`/api/v1/members/${id}`, { method: 'DELETE' }),
};

export const billsApi = {
  list: (page = 1, pageSize = 20, status = '', billingPeriod = '') =>
    apiRequest<any>(`/api/v1/bills?page=${page}&pageSize=${pageSize}&status=${status}&billingPeriod=${billingPeriod}`),
  get: (id: string) => apiRequest<any>(`/api/v1/bills/${id}`),
  generate: (data: any) => apiRequest<any>('/api/v1/bills/generate', { method: 'POST', body: JSON.stringify(data) }),
  bulkGenerate: (data: any) => apiRequest<any>('/api/v1/bills/bulk-generate', { method: 'POST', body: JSON.stringify(data) }),
};

export const paymentsApi = {
  list: (page = 1, pageSize = 20) =>
    apiRequest<any>(`/api/v1/payments?page=${page}&pageSize=${pageSize}`),
  get: (id: string) => apiRequest<any>(`/api/v1/payments/${id}`),
  create: (data: any) => apiRequest<any>('/api/v1/payments', { method: 'POST', body: JSON.stringify(data) }),
  reverse: (id: string, reason: string) =>
    apiRequest<any>(`/api/v1/payments/${id}/reverse`, { method: 'POST', body: JSON.stringify({ reversalReason: reason }) }),
};

export const receiptsApi = {
  list: (page = 1, pageSize = 20) =>
    apiRequest<any>(`/api/v1/receipts?page=${page}&pageSize=${pageSize}`),
  get: (id: string) => apiRequest<any>(`/api/v1/receipts/${id}`),
};

export const chargesApi = {
  list: () => apiRequest<any>('/api/v1/charges'),
  create: (data: any) => apiRequest<any>('/api/v1/charges', { method: 'POST', body: JSON.stringify(data) }),
  update: (id: string, data: any) => apiRequest<any>(`/api/v1/charges/${id}`, { method: 'PUT', body: JSON.stringify(data) }),
  delete: (id: string) => apiRequest<any>(`/api/v1/charges/${id}`, { method: 'DELETE' }),
};

export const tenantsApi = {
  list: () => apiRequest<any>('/api/v1/tenants'),
  get: (id: string) => apiRequest<any>(`/api/v1/tenants/${id}`),
};

export const reportsApi = {
  revenue: (billingPeriod = '') =>
    apiRequest<any>(`/api/v1/reports/revenue?billingPeriod=${billingPeriod}`),
  outstanding: () => apiRequest<any>('/api/v1/reports/outstanding'),
};

export const superAdminApi = {
  dashboard: () => apiRequest<any>('/api/v1/superadmin/dashboard'),
  societies: () => apiRequest<any>('/api/v1/superadmin/societies'),
  createSociety: (data: any) => apiRequest<any>('/api/v1/superadmin/societies', { method: 'POST', body: JSON.stringify(data) }),
};
