// Permission flags
const Permission = {
    None: 0,
    GameAccess: 1,
    Audit: 2,
    Moderate: 4,
    ManageSession: 8,
    Testing: 1024,
    SystemBypass: 536870912,
    ManagePermissions: 1073741824,
    RoleAll: 1152921504606846975
};

// Get all permission names
const PermissionNames = [
    { key: 'GameAccess', value: Permission.GameAccess },
    { key: 'Audit', value: Permission.Audit },
    { key: 'Moderate', value: Permission.Moderate },
    { key: 'ManageSession', value: Permission.ManageSession },
    { key: 'Testing', value: Permission.Testing },
    { key: 'SystemBypass', value: Permission.SystemBypass },
    { key: 'ManagePermissions', value: Permission.ManagePermissions }
];

document.addEventListener('DOMContentLoaded', () => {
    const searchBtn = document.getElementById('searchBtn');
    const searchInput = document.getElementById('searchInput');
    const providerSelect = document.getElementById('providerSelect');
    const usersBody = document.getElementById('usersBody');
    const usersCount = document.getElementById('usersCount');
    const saveStatus = document.getElementById('saveStatus');

    // Handle search
    searchBtn.addEventListener('click', fetchUsers);

    // Allow Enter key to search
    searchInput.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') fetchUsers();
    });

    // Close dropdowns when clicking outside
    document.addEventListener('click', (e) => {
        if (!e.target.closest('.permissions-wrapper')) {
            document.querySelectorAll('.permissions-panel').forEach(panel => {
                panel.classList.remove('open');
            });
        }
    });

    async function fetchUsers() {
        const search = searchInput.value.trim();
        const provider = providerSelect.value;

        try {
            const url = new URL('/api/admin/users', window.location.origin);
            if (search) url.searchParams.append('search', search);
            if (provider) url.searchParams.append('provider', provider);

            const response = await fetch(url.toString());
            if (!response.ok) throw new Error('Ошибка загрузки пользователей');

            const users = await response.json();

            // Clear table
            usersBody.innerHTML = '';

            if (users.length === 0) {
                usersBody.innerHTML = '<tr><td colspan="6" style="text-align: center; color: var(--text-secondary);">Нет результатов</td></tr>';
                usersCount.textContent = '0 пользователей';
                return;
            }

            usersCount.textContent = `${users.length} пользователей`;

            // Render users
            users.forEach(user => {
                const tr = document.createElement('tr');
                tr.dataset.userId = user.id;

                const providerClass = user.provider.toLowerCase();

                tr.innerHTML = `
                    <td class="user-id-cell">${user.id}</td>
                    <td>${escapeHtml(user.username)}</td>
                    <td class="provider-cell">
                        <span class="provider-${providerClass}">&#9632;</span>
                        ${escapeHtml(user.provider)}
                    </td>
                    <td class="user-id-cell">${escapeHtml(user.providerId)}</td>
                    <td>
                        <div class="permissions-wrapper">
                            <button type="button" class="permissions-trigger" data-user-id="${user.id}">Редактировать</button>
                            <div class="permissions-panel" data-user-id="${user.id}">
                                <p class="permissions-label">Выберите права:</p>
                                <div class="permissions-panel-content">
                                    ${generatePermissionCheckboxes(user.permissions)}
                                </div>
                            </div>
                        </div>
                    </td>
                    <td>
                        <button class="save-btn" disabled=true data-user-id="${user.id}">Сохранено</button>
                    </td>
                `;

                usersBody.appendChild(tr);
            });

            // Add event listeners for trigger buttons
            document.querySelectorAll('.permissions-trigger').forEach(trigger => {
                trigger.addEventListener('click', (e) => {
                    e.stopPropagation();
                    const wrapper = trigger.closest('.permissions-wrapper');
                    const panel = wrapper.querySelector('.permissions-panel');
                    panel.classList.toggle('open');
                });
            });

            // Add event listeners for save buttons
            document.querySelectorAll('.save-btn').forEach(btn => {
                btn.addEventListener('click', async (e) => {
                    const tr = btn.closest('tr');
                    const userId = tr.dataset.userId;
                    await savePermissions(userId);
                });
            });

            // Add event listener for checkboxes with event delegation on usersBody
            usersBody.addEventListener('change', (e) => {
                const checkbox = e.target.closest('input[type="checkbox"]');
                if (checkbox) {
                    const tr = checkbox.closest('tr');
                    const btn = tr?.querySelector('.save-btn');
                    if (btn) btn.disabled = false;
                }
            });

        } catch (error) {
            console.error('Error fetching users:', error);
            usersBody.innerHTML = `<tr><td colspan="6" style="text-align: center; color: #f04747;">Ошибка: ${escapeHtml(error.message)}</td></tr>`;
        }
    }

    function generatePermissionCheckboxes(currentPermissions) {
        let html = '';

        PermissionNames.forEach(p => {
            const isChecked = (currentPermissions & p.value) === p.value;
            html += `
                <label class="permission-checkbox">
                    <input type="checkbox" value="${p.value}" ${isChecked ? 'checked' : ''}>
                    <span>${p.key}</span>
                </label>
            `;
        });

        // RoleAll checkbox
        const isRoleAll = currentPermissions === Permission.RoleAll;
        html += `
            <label class="permission-checkbox">
                <input type="checkbox" value="${Permission.RoleAll}" ${isRoleAll ? 'checked' : ''}>
                <span>RoleAll</span>
            </label>
        `;

        return html;
    }

    function getSelectedPermissions(content) {
        const checkboxes = content.querySelectorAll('input[type="checkbox"]:checked');
        let result = 0;

        checkboxes.forEach(checkbox => {
            const value = parseInt(checkbox.value, 10);
            result |= value;
        });

        return result;
    }

    async function savePermissions(userId) {
        const tr = document.querySelector(`tr[data-user-id="${userId}"]`);
        if (!tr) return;

        const wrapper = tr.querySelector('.permissions-wrapper');
        const content = wrapper.querySelector('.permissions-panel-content');
        const btn = tr.querySelector('.save-btn');
        const trigger = wrapper.querySelector('.permissions-trigger');
        if (!btn || !content) return;

        const newPermissions = getSelectedPermissions(content);

        btn.disabled = true;
        btn.textContent = 'Сохранение...';

        try {
            const response = await fetch(`/api/admin/users/${userId}/permissions`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ permissions: newPermissions })
            });

            if (!response.ok) {
                const error = await response.json().catch(() => ({}));
                throw new Error(error.message || 'Ошибка сохранения');
            }

            showSaveStatus('Изменения сохранены!', true);

            // Update button state
            btn.textContent = 'Сохранено';
            btn.disabled = true;

            // Close panel
            wrapper.querySelector('.permissions-panel').classList.remove('open');

        } catch (error) {
            console.error('Error saving permissions:', error);
            showSaveStatus(`Ошибка: ${error.message}`, false);
            btn.disabled = false;
            btn.textContent = 'Сохранить';
        }
    }

    function showSaveStatus(message, isSuccess) {
        saveStatus.textContent = message;
        saveStatus.className = 'save-status ' + (isSuccess ? 'success' : 'error');

        // Clear after 3 seconds
        setTimeout(() => {
            saveStatus.textContent = '';
            saveStatus.className = 'save-status';
        }, 3000);
    }

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Initial fetch on page load
    fetchUsers();
});
