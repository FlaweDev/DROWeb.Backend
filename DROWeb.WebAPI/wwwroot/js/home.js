document.addEventListener('DOMContentLoaded', function () {
    const discordBtn = document.getElementById('discordBtn');
    const avatar = document.getElementById('avatar');
    const username = document.getElementById('username');
    const userId = document.getElementById('userId');
    const adminPanelBtn = document.getElementById('adminPanelBtn');
    const playBtn = document.getElementById('playBtn');
    const logoutBtn = document.getElementById('logoutBtn');
    const statusMessage = document.getElementById('statusMessage');

    // Проверка статуса при загрузке страницы
    checkStatus();

    discordBtn.addEventListener('click', handleDiscordLogin);
    adminPanelBtn.addEventListener('click', handleAdminPanel);
    playBtn.addEventListener('click', handlePlay);
    logoutBtn.addEventListener('click', handleLogout);

    // Константы прав (соответствуют DROWeb.Domain.Models.Permission)
    const PERMISSIONS = {
        GAME_ACCESS: 1 << 0,
        MANAGE_PERMISSIONS: 1 << 30
    };

    async function checkStatus() {
        try {
            const response = await fetch('/api/auth/status', {
                method: 'GET',
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Ошибка проверки статуса');
            }

            const data = await response.json();

            if (data.isAuthenticated) {
                showAuthenticatedPanel(data);
            }
        } catch (error) {
            console.error('Ошибка при проверке статуса:', error);
        }
    }

    function handleDiscordLogin() {
        window.location.href = '/api/auth/login';
    }

    function handlePlay(e) {
        window.location.href = '/game';
    }

    async function showAuthenticatedPanel(data) {
        discordBtn.style.display = 'none';

        avatar.src = await getAvatarUrl(data.userId);
        avatar.classList.remove('hidden');

        username.textContent = data.username;
        username.classList.remove('hidden');
        userId.textContent = `ID: ${data.userId}`;
        userId.classList.remove('hidden');

        const hasManagePermissions = (data.permissions & PERMISSIONS.MANAGE_PERMISSIONS) !== 0;
        if (hasManagePermissions) {
            adminPanelBtn.classList.remove('hidden');
        }

        const hasPlayPermission = (data.permissions & PERMISSIONS.GAME_ACCESS) !== 0;
        if (hasPlayPermission) {
            playBtn.classList.remove('hidden');
            playBtn.disabled = false;
        } else {
            playBtn.classList.remove('hidden');
            playBtn.disabled = true;
            playBtn.textContent = 'Нет доступа';
        }

        logoutBtn.classList.remove('hidden');
    }

    async function getAvatarUrl(userId) {
        if (!userId || userId === '00000000-0000-0000-0000-000000000000') {
            return 'https://cdn.discordapp.com/embed/avatars/0.png';
        }
        try {
            const response = await fetch(`/api/users/${userId}/avatar`, { credentials: 'include' });
            const data = await response.json();
            return data.avatarUrl;
        } catch {
            return 'https://cdn.discordapp.com/embed/avatars/0.png';
        }
    }

    function handleAdminPanel(e) {
        e.preventDefault();
        window.location.href = '/admin';
    }

    async function handleLogout(e) {
        e.preventDefault();

        try {
            await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include'
            });
        } catch (error) {
            console.error('Ошибка при выходе:', error);
        }

        window.location.href = '/';
    }
});
