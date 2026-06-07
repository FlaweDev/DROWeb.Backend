document.addEventListener('DOMContentLoaded', function () {
    const discordBtn = document.getElementById('discordBtn');
    const playBtn = document.getElementById('playBtn');
    const statusMessage = document.getElementById('statusMessage');

    // Проверка статуса при загрузке страницы
    checkStatus();

    // Обработчик кнопки входа через Discord
    discordBtn.addEventListener('click', handleDiscordLogin);

    // Обработчик кнопки "Играть"
    playBtn.addEventListener('click', handlePlay);

    async function checkStatus() {
        try {
            const response = await fetch('/api/auth/status', {
                method: 'GET',
                credentials: 'include' // Важно для передачи cookie
            });

            if (!response.ok) {
                throw new Error('Ошибка проверки статуса');
            }

            const data = await response.json();

            if (data.isAuthenticated) {
                showPlayButton();
                showStatus(`Добро пожаловать, ${data.username}!`, 'success');
            }
        } catch (error) {
            console.error('Ошибка при проверке статуса:', error);
        }
    }

    async function handleDiscordLogin(e) {
        e.preventDefault();

        showStatus('Перенаправление в Discord...', 'info');

        try {
            // Используем POST для инициации OAuth flow
            const response = await fetch('/api/auth/login?returnUrl=' + encodeURIComponent(window.location.href), {
                method: 'POST',
                credentials: 'include'
            });

            if (!response.ok) {
                throw new Error('Ошибка авторизации');
            }

            // Проверка статуса после возврата
            setTimeout(checkStatus, 1000);
        } catch (error) {
            console.error('Ошибка при входе:', error);
            showStatus('Ошибка при входе. Попробуйте снова.', 'error');
        }
    }

    function handlePlay(e) {
        e.preventDefault();

        // Запуск Unity игры
        showStatus('Запуск игры...', 'info');

        // Здесь можно добавить логику запуска Unity WebGL или другого клиента
        setTimeout(() => {
            showStatus('Игра запущена!', 'success');
        }, 1000);
    }

    function showPlayButton() {
        discordBtn.style.display = 'none';
        playBtn.classList.remove('hidden');
    }

    function showStatus(message, type) {
        statusMessage.textContent = message;
        statusMessage.className = 'status-message ' + type;
    }
});
