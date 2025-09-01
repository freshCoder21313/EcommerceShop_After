import { jwtDecode } from 'jwt-decode';
import Swal from 'sweetalert2';
import router from '@/router/index';
import Cookies from 'js-cookie';
import { GetApiUrl } from '@/constants/api';

const getApiUrl = GetApiUrl();

// #region Core Authentication Service
const authService = {
  /**
   * Checks if the current session's access token is expired.
   * If expired, it shows a warning and redirects to the login page.
   * @returns {boolean} True if the token is expired or absent, false otherwise.
   */
  isExpiredSessionAccess() {
    const token = Cookies.get('accessToken');
    if (!token) return true;

    try {
      const decoded = jwtDecode(token);
      const currentTime = Date.now() / 1000;

      if (decoded.exp < currentTime) {
        Swal.fire({
          title: 'Phiên đăng nhập đã hết hạn',
          text: 'Vui lòng đăng nhập lại để tiếp tục sử dụng dịch vụ của chúng tôi.',
          icon: 'warning',
          confirmButtonText: 'Đăng nhập lại',
          allowOutsideClick: false,
        }).then((result) => {
          if (result.isConfirmed) {
            router.push('/Login');
          }
        });
        return true;
      }
      return false;
    } catch (error) { // eslint-disable-line no-unused-vars
      console.error('Error decoding token:', error);
      return true; // Treat decoding errors as an expired session
    }
  },

  /**
   * Checks if the user is authenticated by verifying the access token.
   * @returns {boolean} True if the token is valid and not expired, false otherwise.
   */
  isAuthenticated() {
    const token = Cookies.get('accessToken');
    if (!token) return false;

    try {
      const decoded = jwtDecode(token);
      const currentTime = Date.now() / 1000;
      return decoded.exp >= currentTime;
    } catch (error) { // eslint-disable-line no-unused-vars
      return false; // Invalid token
    }
  },
};
// #endregion

// #region Token Management

/**
 * Decodes the JWT token to extract user information.
 * @param {string} token - The JWT access token.
 * @returns {object|null} The decoded token payload or null if the token is invalid.
 */
export function decodeToken(token) {
  if (!token) return null;
  try {
    const decoded = jwtDecode(token);
    return {
      IdUser: decoded.sub,
      Phone: decoded.PhoneNumber,
      Name: decoded.FullName,
      Role: decoded.role,
      Exp: decoded.exp,
    };
  } catch (error) {
    console.error('Failed to decode token:', error);
    return null;
  }
}

/**
 * Renews the access token using the refresh token.
 * @returns {Promise<string>} The new access token.
 * @throws Will throw an error if the refresh token is missing or the renewal fails.
 */
export const refreshToken = async () => {
  const refreshTokenValue = Cookies.get('refreshToken');
  if (!refreshTokenValue) {
    throw new Error('Không có refresh token');
  }

  try {
    const response = await fetch(`${getApiUrl}/api/Account/RenewAccessToken`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken: refreshTokenValue }),
    });

    if (!response.ok) {
      throw new Error('Không thể làm mới token');
    }

    const data = await response.json();
    if (data.success) {
      const { accessToken, refreshToken: newRefreshToken } = data.data;
      Cookies.set('accessToken', accessToken, { expires: 3 / 24 });
      Cookies.set('refreshToken', newRefreshToken, { expires: 3 / 24 });
      return accessToken;
    } else {
      throw new Error(data.message || 'Làm mới token thất bại');
    }
  } catch (error) {
    console.error('Lỗi refresh token:', error);
    Cookies.remove('accessToken');
    Cookies.remove('refreshToken');
    throw error;
  }
};

/**
 * Validates the current access token. If it's expired, it tries to renew it.
 * @returns {Promise<{isValid: boolean, newAccessToken?: string}>} An object indicating validity and the new token if renewed.
 */
export async function validateToken() {
  const accessToken = Cookies.get('accessToken');
  const refreshTokenValue = Cookies.get('refreshToken');

  if (!accessToken || !refreshTokenValue) {
    Cookies.remove('accessToken');
    Cookies.remove('refreshToken');
    return { isValid: false };
  }

  const decodedToken = decodeToken(accessToken);
  if (!decodedToken) {
    Cookies.remove('accessToken');
    Cookies.remove('refreshToken');
    return { isValid: false };
  }

  const currentTime = Math.floor(Date.now() / 1000);
  if (decodedToken.Exp < currentTime) {
    try {
      const newAccessToken = await refreshToken();
      return { isValid: true, newAccessToken };
    } catch (error) { // eslint-disable-line no-unused-vars
      return { isValid: false };
    }
  }

  return { isValid: true, newAccessToken: accessToken };
}

/**
 * Retrieves user information from the access token stored in cookies.
 * @returns {object|null} User information object or null if not available.
 */
export function getUserInfo() {
  const token = Cookies.get('accessToken');
  return token ? decodeToken(token) : null;
}

// #endregion

// #region API Fetch Utility

/**
 * A wrapper for the fetch API that automatically includes the Authorization header
 * and handles token refreshing.
 * @param {string} url - The URL to fetch.
 * @param {object} options - The options for the fetch request.
 * @returns {Promise<Response>} The fetch response.
 */
export const fetchWithAuth = async (url, options = {}) => {
  try {
    let accessToken = Cookies.get('accessToken');

    if (!accessToken) {
      throw new Error('Chưa đăng nhập');
    }

    // Add authorization header
    const headers = {
      ...options.headers,
      Authorization: `Bearer ${accessToken}`,
    };

    // Initial API call
    let response = await fetch(url, { ...options, headers });

    // If token expired (401), try to refresh and retry
    if (response.status === 401) {
      const newAccessToken = await refreshToken();
      const newHeaders = {
        ...options.headers,
        Authorization: `Bearer ${newAccessToken}`,
      };
      response = await fetch(url, { ...options, headers: newHeaders });
    }

    return response;
  } catch (error) {
    console.error('Lỗi khi gọi API:', error);
    // Redirect to login if refresh fails or other auth errors occur
    if (error.message.includes('hết hạn') || error.message.includes('refresh token')) {
       router.push('/Login');
    }
    throw error;
  }
};

// #endregion

export default authService;