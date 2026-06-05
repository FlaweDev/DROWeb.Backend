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


        if (data.isAdmin) {
            adminPanelBtn.classList.remove('hidden');
        }
        playBtn.classList.remove('hidden');
        logoutBtn.classList.remove('hidden');
    }

    async function getAvatarUrl(userId) {
        if (!userId || userId === '00000000-0000-0000-0000-000000000000') {
            return 'https://cdn.discordapp.com/embed/avatars/0.png';
        }
        try {
            const response = await fetch(`/users/${userId}/avatar`, { credentials: 'include' });
            const data = await response.json();
            return data.avatarUrl;
        } catch {
            return 'https://cdn.discordapp.com/embed/avatars/0.png';
        }
    }

    function handleAdminPanel(e) {
        e.preventDefault();
        window.location.href = '/control';
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
