import { apiFetch, setAuthTokens, clearAuthTokens } from './client';
import type { AuthResponse } from '../types/api';

export async function login(email: string, password: string) {
    const res = await apiFetch<AuthResponse>('/api/auth/login', {
        method: 'POST',
        body: JSON.stringify({ email, password }),
    });
    if (res.ok && res.data?.success && res.data.data) {
        setAuthTokens(res.data.data.accessToken, res.data.data.refreshToken);
        return { success: true, data: res.data.data };
    }
    return {
        success: false,
        error: (res as { problem?: { detail?: string } }).problem?.detail ?? res.data?.message ?? 'Login failed',
    };
}

export async function register(payload: {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
}) {
    const res = await apiFetch<AuthResponse>('/api/auth/register', {
        method: 'POST',
        body: JSON.stringify(payload),
    });
    if (res.ok && res.data?.success && res.data.data) {
        setAuthTokens(res.data.data.accessToken, res.data.data.refreshToken);
        return { success: true, data: res.data.data };
    }
    return {
        success: false,
        error: (res as { problem?: { detail?: string } }).problem?.detail ?? res.data?.message ?? 'Registration failed',
    };
}

export async function revokeRefresh(refreshTokenValue: string) {
    await apiFetch('/api/auth/revoke', {
        method: 'POST',
        body: JSON.stringify({ refreshToken: refreshTokenValue }),
    });
    clearAuthTokens();
}