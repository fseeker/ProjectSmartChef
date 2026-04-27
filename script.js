// Navigation
function switchTab(tabId) {
    document.querySelectorAll('.tab-content').forEach(el => el.classList.remove('active'));
    document.querySelectorAll('.nav-links a').forEach(el => el.classList.remove('active'));
    document.getElementById('tab-' + tabId).classList.add('active');

    if (tabId !== 'home') {
        const links = document.querySelectorAll('.nav-links a');
        for (let link of links) {
            if (link.getAttribute('onclick') === `switchTab('${tabId}')`) link.classList.add('active');
        }
    }
    window.scrollTo(0, 0);
}

// ---------------- LOGIN & AUTH ----------------
let currentUser = null;
const API_BASE = '/api';

function openLogin() {
    console.log("Opening login modal");
    document.getElementById('loginModal').style.display = 'flex';
    toggleAuthMode('login');
}
function closeLogin() { document.getElementById('loginModal').style.display = 'none'; }

function toggleAuthMode(mode) {
    console.log("toggleAuthMode called with:", mode);
    const loginSec = document.getElementById('loginFormSection');
    const signupSec = document.getElementById('signupFormSection');

    if (!loginSec || !signupSec) {
        console.error("Critical Error: Auth sections not found in DOM");
        return;
    }

    if (mode === 'signup') {
        loginSec.style.display = 'none';
        signupSec.style.display = 'block';
    } else {
        loginSec.style.display = 'block';
        signupSec.style.display = 'none';
    }
}

// ---------------- CART LOGIC ----------------
// hi Sinem I see your work I ove you
let cart = [];
function addToCart(name, priceOrBtn, amount = 1) {
    let price = 0;
    if (typeof priceOrBtn === 'number') {
        price = priceOrBtn;
    } else if (typeof priceOrBtn === 'string') {
        price = parseFloat(priceOrBtn);
    } else {
        price = 5.99; // fallback
    }

    cart.push({ name, price, amount: parseFloat(amount) });
    document.getElementById('cartCount').innerText = cart.length;

    // Visual button feedback if called from a click event
    if (event && event.target && event.target.tagName === 'BUTTON') {
        const btn = event.target;
        const ogText = btn.innerText;
        btn.innerText = 'Added!';
        btn.style.background = 'var(--secondary-color)';
        btn.style.color = '#000';
        setTimeout(() => {
            btn.innerText = ogText;
            btn.style.background = '';
            btn.style.color = '';
        }, 1000);
    }
}

function addMultipleToCart(productsStr, btnElement) {
    let products = [];
    try {
        products = JSON.parse(decodeURIComponent(productsStr));
    } catch (e) {
        console.error("Failed to parse products for cart", e);
        return;
    }
    
    products.forEach(p => {
        cart.push({ name: p.name, price: p.price, amount: 1 });
    });
    document.getElementById('cartCount').innerText = cart.length;

    if (btnElement) {
        const ogText = btnElement.innerHTML;
        btnElement.innerHTML = '<i class="fa-solid fa-check"></i> Added!';
        btnElement.style.background = 'var(--secondary-color)';
        btnElement.style.color = '#000';
        setTimeout(() => {
            btnElement.innerHTML = ogText;
            btnElement.style.background = '';
            btnElement.style.color = '';
        }, 1000);
    }
}

function generateAmountOptions(allowedAmounts) {
    if (!allowedAmounts) return '<option value="1">1</option>';
    let options = '';
    if (allowedAmounts.includes('-')) {
        const parts = allowedAmounts.split('-');
        const min = parseInt(parts[0]);
        const max = parseInt(parts[1]);
        for (let i = min; i <= Math.min(max, 30); i++) {
            options += `<option value="${i}">${i}</option>`;
        }
    } else if (allowedAmounts.includes(',')) {
        const parts = allowedAmounts.split(',');
        parts.forEach(p => {
            options += `<option value="${p.trim()}">${p.trim()}</option>`;
        });
    } else {
        options = `<option value="${allowedAmounts}">${allowedAmounts}</option>`;
    }
    return options;
}

function openCart() {
    const list = document.getElementById('cartItemList');
    let total = 0;
    list.innerHTML = '';

    if (cart.length === 0) {
        list.innerHTML = '<p style="color: var(--text-muted);">Cart is empty.</p>';
    } else {
        cart.forEach(item => {
            list.innerHTML += `<div class="cart-item"><span>${item.name}</span><span>$${item.price.toFixed(2)}</span></div>`;
            total += item.price;
        });
    }
    document.getElementById('cartTotal').innerText = `$${total.toFixed(2)}`;
    document.getElementById('cartModal').style.display = 'flex';
}
function closeCart() { document.getElementById('cartModal').style.display = 'none'; }


let pantryUnits = [];
let selectedPantryProductId = null;

async function fetchUnits() {
    try {
        const res = await fetch(`${API_BASE}/lookup/units`);
        if (res.ok) {
            pantryUnits = await res.json();
            const select = document.getElementById('pantryUnitSelect');
            if (select) {
                select.innerHTML = '<option value="">Select Unit</option>';
                pantryUnits.forEach(u => {
                    select.innerHTML += `<option value="${u.id}">${u.name}</option>`;
                });
            }
        }
    } catch (err) {
        console.error("Failed to fetch units:", err);
    }
}
fetchUnits();

function searchPantryProducts() {
    const term = document.getElementById('pantryInput').value.toLowerCase();
    const resultsContainer = document.getElementById('pantrySearchResults');

    selectedPantryProductId = null; // reset if they type
    document.getElementById('pantryUnitSelect').disabled = false;

    if (term.length < 2) {
        resultsContainer.style.display = 'none';
        return;
    }

    const filtered = liveProducts.filter(p => p.title.toLowerCase().includes(term)).slice(0, 5);

    if (filtered.length === 0) {
        resultsContainer.style.display = 'none';
        return;
    }

    resultsContainer.innerHTML = '';
    filtered.forEach(p => {
        const div = document.createElement('div');
        div.className = 'search-result-item';
        div.innerHTML = `<strong>${p.title}</strong> <span style="font-size:0.8rem; color:var(--text-muted);">${p.brand}</span>`;
        div.onclick = () => selectPantryProduct(p);
        resultsContainer.appendChild(div);
    });
    resultsContainer.style.display = 'block';
}

function selectPantryProduct(product) {
    document.getElementById('pantryInput').value = product.title;
    selectedPantryProductId = product.id;
    document.getElementById('pantrySearchResults').style.display = 'none';

    // Auto-select unit from product
    const unitSelect = document.getElementById('pantryUnitSelect');
    if (product.unitId) {
        unitSelect.value = product.unitId;
        unitSelect.disabled = true; // lock unit for known products
    }
}

async function processLogin() {
    const userOrEmail = document.getElementById('loginUsername').value.trim();
    const pass = document.getElementById('loginPassword').value;

    if (!userOrEmail || !pass) {
        alert("Please enter both username/email and password.");
        return;
    }

    try {
        const res = await fetch(`${API_BASE}/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ usernameOrEmail: userOrEmail, password: pass })
        });

        const data = await res.json();
        if (res.ok) {
            currentUser = data;
            localStorage.setItem('smartchef_user', JSON.stringify(currentUser));
            updateUIForLoggedInUser();
            closeLogin();
            loadPantry();
        } else {
            alert(data.message || "Login failed.");
        }
    } catch (err) {
        console.error(err);
        alert("Server error during login.");
    }
}

function updateUIForLoggedInUser() {
    const user = currentUser;
    if (!user) return;

    // Show profile link
    const profileNavLink = document.getElementById('profileNavLink');
    if (profileNavLink) profileNavLink.style.display = 'inline-block';

    // Swap login btn for user menu
    const loginBtn = document.getElementById('loginHeaderBtn');
    if (loginBtn) loginBtn.style.display = 'none';
    const userMenu = document.getElementById('userMenu');
    if (userMenu) userMenu.style.display = 'flex';

    // Populate user name in the button
    const userGreeting = document.getElementById('userGreeting');
    if (userGreeting) userGreeting.textContent = user.username || user.fullName || 'User';

    // Profile hero
    const profileHeroName = document.getElementById('profileHeroName');
    if (profileHeroName) profileHeroName.textContent = `${user.fullName || user.username}'s Profile`;
    const profileHeroMeta = document.getElementById('profileHeroMeta');
    if (profileHeroMeta) profileHeroMeta.textContent = `Skill Level ${user.cookingSkill || '?'}/10 · SmartChef Member`;

    // Pre-fill profile fields
    const profileFullName = document.getElementById('profileFullName');
    if (profileFullName) profileFullName.value = user.fullName || '';
    const profileUsername = document.getElementById('profileUsername');
    if (profileUsername) profileUsername.value = user.username || '';
    const profileEmail = document.getElementById('profileEmail');
    if (profileEmail) profileEmail.value = user.email || '';
    const profileBio = document.getElementById('profileBio');
    if (profileBio) profileBio.value = user.bio || '';
    if (user.cookingSkill) {
        const profileCookingSkill = document.getElementById('profileCookingSkill');
        if (profileCookingSkill) profileCookingSkill.value = user.cookingSkill;
        const profileSkillVal = document.getElementById('profileSkillVal');
        if (profileSkillVal) profileSkillVal.textContent = user.cookingSkill;
    }

    // Check for onboarding
    if (localStorage.getItem('show_onboarding') === 'true') {
        localStorage.removeItem('show_onboarding');
        switchTab('profile');
        const onboardingHeader = document.getElementById('onboardingHeader');
        if (onboardingHeader) onboardingHeader.style.display = 'block';
    }

    if (typeof loadUserProfile === 'function') {
        loadUserProfile();
    }
}

function toggleUserDropdown() {
    const dropdown = document.getElementById('userDropdown');
    const chevron = document.getElementById('userMenuChevron');
    dropdown.classList.toggle('open');
    chevron.classList.toggle('open');
}

// Close dropdown when clicking outside
document.addEventListener('click', (e) => {
    const menu = document.getElementById('userMenu');
    if (menu && !menu.contains(e.target)) {
        document.getElementById('userDropdown')?.classList.remove('open');
        document.getElementById('userMenuChevron')?.classList.remove('open');
    }
});

function processLogout() {
    currentUser = null;
    localStorage.removeItem('smartchef_user');
    document.getElementById('loginHeaderBtn').style.display = '';
    document.getElementById('userMenu').style.display = 'none';
    document.getElementById('userDropdown').classList.remove('open');
    switchTab('home');
}

// -------- PROFILE PAGE --------
function switchProfileTab(name, btn) {
    document.querySelectorAll('.profile-section').forEach(s => s.classList.remove('active'));
    document.querySelectorAll('.profile-tab').forEach(b => b.classList.remove('active'));
    document.getElementById(`profile-${name}`).classList.add('active');
    btn.classList.add('active');

    // Lazy-load taste sliders when switching to that tab
    if (name === 'taste') loadProfileTasteSliders();
}

async function loadProfileTasteSliders() {
    const container = document.getElementById('profileTasteSliders');
    if (container.dataset.loaded === '1') return;
    try {
        const res = await fetch(`${API_BASE}/lookup/tastes`);
        const tastes = await res.json();
        const labels = { 1: 'Hate', 2: 'Dislike', 3: 'Neutral', 4: 'Like', 5: 'Love' };
        container.innerHTML = '';
        tastes.forEach(t => {
            container.innerHTML += `
            <div class="taste-slider-group">
                <label><span>${t.name}</span> <span id="ptaste-val-${t.id}" style="color:var(--primary-color);font-weight:800;">Neutral</span></label>
                <input type="range" id="ptaste-${t.id}" min="1" max="5" value="3" oninput="updateProfileTasteVal(${t.id}, this.value)">
            </div>`;
        });
        container.dataset.loaded = '1';
        if (typeof applyTastePalatesToSliders === 'function') applyTastePalatesToSliders();
    } catch (e) { console.error(e); }
}

function updateProfileTasteVal(id, val) {
    const labels = { 1: 'Hate', 2: 'Dislike', 3: 'Neutral', 4: 'Like', 5: 'Love' };
    document.getElementById(`ptaste-val-${id}`).textContent = labels[val];
}

async function saveProfileAccount() {
    if (!currentUser) return;
    const fullName = document.getElementById('profileFullName').value.trim();
    const bio = document.getElementById('profileBio').value.trim();
    try {
        const res = await fetch(`${API_BASE}/user/${currentUser.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName, bio })
        });
        if (res.ok) {
            currentUser.fullName = fullName;
            currentUser.bio = bio;
            localStorage.setItem('smartchef_user', JSON.stringify(currentUser));
            updateUIForLoggedInUser();
            alert('Profile updated!');
        } else { alert('Failed to save.'); }
    } catch (e) { alert('Server error.'); }
}

async function saveProfileCooking() {
    if (!currentUser) return;
    const cookingSkill = parseInt(document.getElementById('profileCookingSkill').value);
    try {
        const res = await fetch(`${API_BASE}/user/${currentUser.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ cookingSkill })
        });
        if (res.ok) {
            currentUser.cookingSkill = cookingSkill;
            localStorage.setItem('smartchef_user', JSON.stringify(currentUser));
            updateUIForLoggedInUser();
            alert('Cooking skill updated!');
        } else { alert('Failed to save.'); }
    } catch (e) { alert('Server error.'); }
}

async function saveProfileAllergies() { alert('Allergy save coming soon — requires additional API endpoint.'); }
async function saveProfileCuisines() { alert('Cuisine save coming soon — requires additional API endpoint.'); }
async function saveProfileTastes() { alert('Taste profile save coming soon — requires additional API endpoint.'); }

async function searchProfileAllergies() {
    const term = document.getElementById('profileAllergySearch').value.trim().toLowerCase();
    const results = document.getElementById('profileAllergyResults');
    if (term.length < 2) { results.style.display = 'none'; return; }
    try {
        const res = await fetch(`${API_BASE}/products?category=all`);
        const data = await res.json();
        const filtered = data.filter(p => p.title.toLowerCase().includes(term) && p.isVisible).slice(0, 5);
        results.innerHTML = '';
        filtered.forEach(p => { results.innerHTML += `<div class="search-item" onclick="addProfileAllergyTag(${p.id}, '${p.title}')">${p.title}</div>`; });
        if (!filtered.some(p => p.title.toLowerCase() === term)) {
            results.innerHTML += `<div class="search-item" style="color:var(--secondary-color);" onclick="addProfileAllergyTag(null, '${term}')">+ Add "${term}"</div>`;
        }
        results.style.display = 'block';
    } catch (e) { console.error(e); }
}

let profileAllergies = [];
function addProfileAllergyTag(id, title) {
    if (profileAllergies.find(a => a.title === title)) return;
    profileAllergies.push({ id, title });
    renderProfileAllergyTags();
    document.getElementById('profileAllergySearch').value = '';
    document.getElementById('profileAllergyResults').style.display = 'none';
}
function removeProfileAllergyTag(title) {
    profileAllergies = profileAllergies.filter(a => a.title !== title);
    renderProfileAllergyTags();
}
function renderProfileAllergyTags() {
    const c = document.getElementById('profileAllergyTags');
    c.innerHTML = '';
    profileAllergies.forEach(a => { c.innerHTML += `<div class="tag">${a.title} <i class="fa-solid fa-xmark" onclick="removeProfileAllergyTag('${a.title}')"></i></div>`; });
}

async function searchProfileCuisines() {
    const term = document.getElementById('profileCuisineSearch').value.trim().toLowerCase();
    const results = document.getElementById('profileCuisineResults');
    if (term.length < 2) { results.style.display = 'none'; return; }
    try {
        const res = await fetch(`${API_BASE}/lookup/cuisines`);
        const data = await res.json();
        const filtered = data.filter(c => c.name.toLowerCase().includes(term)).slice(0, 5);
        results.innerHTML = '';
        filtered.forEach(c => { results.innerHTML += `<div class="search-item" onclick="addProfileCuisineTag(${c.id}, '${c.name}')">${c.name}</div>`; });
        results.style.display = 'block';
    } catch (e) { console.error(e); }
}

let profileCuisines = [];
function addProfileCuisineTag(id, name) {
    if (profileCuisines.find(c => c.name === name)) return;
    profileCuisines.push({ id, name });
    renderProfileCuisineTags();
    document.getElementById('profileCuisineSearch').value = '';
    document.getElementById('profileCuisineResults').style.display = 'none';
}
function removeProfileCuisineTag(name) {
    profileCuisines = profileCuisines.filter(c => c.name !== name);
    renderProfileCuisineTags();
}
function renderProfileCuisineTags() {
    const c = document.getElementById('profileCuisineTags');
    c.innerHTML = '';
    profileCuisines.forEach(ci => { c.innerHTML += `<div class="tag">${ci.name} <i class="fa-solid fa-xmark" onclick="removeProfileCuisineTag('${ci.name}')"></i></div>`; });
}

// Restore session on page load
window.addEventListener('load', () => {
    const saved = localStorage.getItem('smartchef_user');
    if (saved) {
        currentUser = JSON.parse(saved);
        updateUIForLoggedInUser();
    }
});

// ---------------- MULTI-STEP SIGNUP LOGIC ----------------
let signupStep = 1;
let selectedAllergies = [];
let selectedCuisines = [];
let tastePalates = [];

function nextSignupStep(current, direction = 1) {
    if (direction === 1) {
        // Validation
        if (current === 1) {
            const username = document.getElementById('signupUsername').value.trim();
            const email = document.getElementById('signupEmail').value.trim();
            const password = document.getElementById('signupPassword').value;
            if (!username || !email || !password) {
                alert("Please fill in all account fields.");
                return;
            }
        }
        if (current === 3 && tastePalates.length === 0) {
            loadTastePalates();
        }
    }

    const next = current + direction;
    if (next < 1 || next > 5) return;

    // UI Toggle
    document.querySelectorAll('.signup-step-content').forEach(el => el.classList.remove('active'));
    document.getElementById(`signup-step-${next}`).classList.add('active');

    // Progress Toggle
    document.querySelectorAll('.step-dot').forEach((dot, idx) => {
        const stepNum = idx + 1;
        dot.classList.remove('active', 'completed');
        if (stepNum < next) dot.classList.add('completed');
        if (stepNum === next) dot.classList.add('active');
    });

    // Progress Bar Fill
    const progressFill = document.getElementById('progress-fill');
    const percent = ((next - 1) / 4) * 100;
    progressFill.style.width = `${percent}%`;

    signupStep = next;
}

// ... (search functions)
async function searchSignupAllergies() {
    const term = document.getElementById('allergySearchInput').value.trim().toLowerCase();
    const results = document.getElementById('allergySearchResults');
    if (term.length < 2) { results.style.display = 'none'; return; }

    try {
        const res = await fetch(`${API_BASE}/products?category=all`);
        const data = await res.json();
        const filtered = data.filter(p => p.title.toLowerCase().includes(term) && p.isVisible).slice(0, 5);

        results.innerHTML = '';
        filtered.forEach(p => {
            results.innerHTML += `<div class="search-item" onclick="addAllergy(${p.id}, '${p.title}')">${p.title}</div>`;
        });

        // Add custom option if no exact match
        if (!filtered.some(p => p.title.toLowerCase() === term)) {
            results.innerHTML += `<div class="search-item" style="color: var(--secondary-color);" onclick="addAllergy(null, '${term}')">+ Add "${term}" as custom allergy</div>`;
        }

        results.style.display = 'block';
    } catch (err) { console.error(err); }
}

function addAllergy(id, title) {
    if (selectedAllergies.find(a => a.title === title)) return;
    selectedAllergies.push({ id, title });
    renderAllergyTags();
    document.getElementById('allergySearchInput').value = '';
    document.getElementById('allergySearchResults').style.display = 'none';
}

function removeAllergy(title) {
    selectedAllergies = selectedAllergies.filter(a => a.title !== title);
    renderAllergyTags();
}

function renderAllergyTags() {
    const container = document.getElementById('selectedAllergies');
    container.innerHTML = '';
    selectedAllergies.forEach(a => {
        container.innerHTML += `<div class="tag">${a.title} <i class="fa-solid fa-xmark" onclick="removeAllergy('${a.title}')"></i></div>`;
    });
}

async function searchSignupCuisines() {
    const term = document.getElementById('cuisineSearchInput').value.trim().toLowerCase();
    const results = document.getElementById('cuisineSearchResults');
    if (term.length < 2) { results.style.display = 'none'; return; }

    try {
        const res = await fetch(`${API_BASE}/lookup/cuisines`);
        const data = await res.json();
        const filtered = data.filter(c => c.name.toLowerCase().includes(term)).slice(0, 5);

        results.innerHTML = '';
        filtered.forEach(c => {
            results.innerHTML += `<div class="search-item" onclick="addCuisine(${c.id}, '${c.name}')">${c.name} (${c.countryOfOrigin})</div>`;
        });
        results.style.display = 'block';
    } catch (err) { console.error(err); }
}

function addCuisine(id, name) {
    if (selectedCuisines.find(c => c.name === name)) return;
    selectedCuisines.push({ id, name });
    renderCuisineTags();
    document.getElementById('cuisineSearchInput').value = '';
    document.getElementById('cuisineSearchResults').style.display = 'none';
}

function removeCuisine(name) {
    selectedCuisines = selectedCuisines.filter(c => c.name !== name);
    renderCuisineTags();
}

function renderCuisineTags() {
    const container = document.getElementById('selectedCuisines');
    container.innerHTML = '';
    selectedCuisines.forEach(c => {
        container.innerHTML += `<div class="tag">${c.name} <i class="fa-solid fa-xmark" onclick="removeCuisine('${c.name}')"></i></div>`;
    });
}

async function loadTastePalates() {
    try {
        const res = await fetch(`${API_BASE}/lookup/tastes`);
        tastePalates = await res.json();
        const container = document.getElementById('tastePalateContainer');
        container.innerHTML = '';

        const labels = {
            1: "Hate",
            2: "Dislike",
            3: "Neutral",
            4: "Like",
            5: "Love"
        };

        tastePalates.forEach(t => {
            container.innerHTML += `
                <div class="taste-slider-group">
                    <label><span>${t.name}</span> <span id="taste-val-${t.id}" style="color: var(--primary-color); font-weight: 800;">Neutral</span></label>
                    <input type="range" min="1" max="5" value="3" oninput="updateTasteVal(${t.id}, this.value)">
                </div>
            `;
        });
    } catch (err) { console.error(err); }
}

function updateTasteVal(id, val) {
    const labels = {
        1: "Hate",
        2: "Dislike",
        3: "Neutral",
        4: "Like",
        5: "Love"
    };
    document.getElementById(`taste-val-${id}`).innerText = labels[val];
}

async function processSignup() {
    const fullName = document.getElementById('signupFullName').value.trim();
    const username = document.getElementById('signupUsername').value.trim();
    const email = document.getElementById('signupEmail').value.trim();
    const password = document.getElementById('signupPassword').value;
    const bio = document.getElementById('signupBio').value.trim();
    const cookingSkill = parseInt(document.getElementById('signupCookingSkill').value);

    const labels = { "Hate": 1, "Dislike": 2, "Neutral": 3, "Like": 4, "Love": 5 };

    const onboardingData = {
        fullName, username, email, password, bio, cookingSkill,
        cuisineIds: selectedCuisines.map(c => c.id),
        allergyIds: selectedAllergies.filter(a => a.id !== null).map(a => a.id),
        customAllergies: selectedAllergies.filter(a => a.id === null).map(a => a.title),
        tasteRatings: {}
    };

    tastePalates.forEach(t => {
        const label = document.getElementById(`taste-val-${t.id}`).innerText;
        onboardingData.tasteRatings[t.id] = labels[label];
    });

    try {
        const res = await fetch(`${API_BASE}/auth/register`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(onboardingData)
        });

        const data = await res.json();
        if (res.ok) {
            alert("Welcome to SmartChef Premium! Your profile is personalized.");
            localStorage.setItem('show_onboarding', 'true');
            // Auto login
            document.getElementById('loginUsername').value = username;
            document.getElementById('loginPassword').value = password;
            await processLogin();
        } else {
            alert(data.message || "Signup failed.");
        }
    } catch (err) {
        console.error(err);
        alert("Server error during signup.");
    }
}



async function loadUserProfile() {
    if (!currentUser) return;
    try {
        const res = await fetch(`${API_BASE}/user/${currentUser.id}`);
        const data = await res.json();
        if (res.ok) {
            document.getElementById('profileFullName').value = data.fullName || '';
            document.getElementById('profileBio').value = data.bio || '';

            const userTypeField = document.getElementById('profileUserType');
            if (userTypeField) userTypeField.value = data.userTypeName || 'Customer';

            if (data.allergies) {
                profileAllergies = data.allergies.map(a => ({ id: a.id, title: a.title }));
                renderProfileAllergyTags();
            }

            if (data.favoriteCuisines) {
                profileCuisines = data.favoriteCuisines.map(c => ({ id: c.id, name: c.name }));
                renderProfileCuisineTags();
            }

            if (data.tastePalates) {
                window.loadedTastePalates = data.tastePalates;
                if (document.getElementById('profileTasteSliders').dataset.loaded === '1') {
                    applyTastePalatesToSliders();
                }
            }
        }
    } catch (err) {
        console.error("Error loading profile:", err);
    }
}

function applyTastePalatesToSliders() {
    if (window.loadedTastePalates) {
        window.loadedTastePalates.forEach(tp => {
            const slider = document.getElementById(`ptaste-${tp.tasteId}`);
            if (slider) {
                slider.value = tp.rating;
                updateProfileTasteVal(tp.tasteId, tp.rating);
            }
        });
    }
}

async function saveUserProfile() {
    if (!currentUser) return;

    const fullName = document.getElementById('profileFullName').value;
    const bio = document.getElementById('profileBio').value;
    const dietaryPreferences = document.getElementById('profileDietary').value;
    const favoriteCuisines = document.getElementById('profileCuisines').value;

    try {
        const res = await fetch(`${API_BASE}/user/${currentUser.id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ fullName, bio, dietaryPreferences, favoriteCuisines })
        });

        if (res.ok) {
            alert("Profile updated successfully!");
            document.getElementById('onboardingHeader').style.display = 'none';
        } else {
            alert("Failed to update profile.");
        }
    } catch (err) {
        console.error("Error saving profile:", err);
        alert("Server error saving profile.");
    }
}

// Auto-login if previously saved
window.onload = function () {
    const saved = localStorage.getItem('smartchef_user');
    if (saved) {
        currentUser = JSON.parse(saved);
        updateUIForLoggedInUser();
        loadPantry();
    } else {
        fetchLiveProducts();
    }
}

// ---------------- PANTRY LOGIC ----------------
let pantryItems = [];
async function addPantryItem() {
    if (!currentUser) {
        alert("Please log in to manage your pantry.");
        return;
    }

    const nameInput = document.getElementById('pantryInput').value.trim();
    const amount = parseFloat(document.getElementById('pantryAmountInput').value) || 0;
    const unitId = parseInt(document.getElementById('pantryUnitSelect').value) || 0;

    if (!nameInput) {
        alert("Please enter a name or select a product.");
        return;
    }

    if (amount <= 0) {
        alert("Please enter a valid quantity.");
        return;
    }

    if (unitId === 0) {
        alert("Please select a unit.");
        return;
    }

    const newItem = {
        name: selectedPantryProductId ? null : nameInput,
        productId: selectedPantryProductId,
        amount: amount,
        unitId: unitId
    };

    try {
        const res = await fetch(`${API_BASE}/pantry/${currentUser.id}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newItem)
        });
        if (res.ok) {
            loadPantry();
            document.getElementById('pantryInput').value = '';
            document.getElementById('pantryAmountInput').value = '';
            document.getElementById('pantryUnitSelect').value = '';
            document.getElementById('pantryUnitSelect').disabled = false;
            selectedPantryProductId = null;
            document.getElementById('pantrySearchResults').style.display = 'none';
        } else {
            alert("Failed to save to cloud pantry.");
        }
    } catch (err) {
        console.error("Error adding pantry item:", err);
    }
}

function renderPantry() {
    const list = document.getElementById('pantryList');
    list.innerHTML = '';
    if (pantryItems.length === 0) {
        list.innerHTML = '<p style="color: var(--text-muted);">Your pantry is empty.</p>';
        return;
    }
    pantryItems.forEach(item => {
        const amountStr = item.amount ? ` <span style="color:var(--text-muted); font-size: 0.9rem;">- ${item.amount} ${item.unitName}</span>` : '';
        list.innerHTML += `<span class="pantry-item">${item.name}${amountStr}</span>`;
    });
}

function savePantryLocal() {
    if (currentUser) {
        localStorage.setItem(`pantry_${currentUser.id}`, JSON.stringify(pantryItems));
    }
}

async function loadPantry() {
    if (currentUser) {
        try {
            const res = await fetch(`${API_BASE}/pantry/${currentUser.id}`);
            if (res.ok) {
                pantryItems = await res.json();
                renderPantry();
            } else {
                pantryItems = [];
                renderPantry();
            }
        } catch (err) {
            console.error(err);
            pantryItems = [];
            renderPantry();
        }
    } else {
        pantryItems = [];
        renderPantry();
    }
    fetchLiveProducts();
}

function suggestRecipes() {
    if (pantryItems.length === 0) { alert("Add items to your pantry first!"); return; }
    switchTab('whattocook');
    const names = pantryItems.map(item => typeof item === 'string' ? item : item.name);
    document.getElementById('aiPantryInput').value = names.join(', ');
    generateAIRecipes();
}

// ---------------- LOCAL DATABASE PRODUCTS LOGIC (SHOP) ----------------
let currentCategory = 'all';
let liveProducts = [];

async function fetchLiveProducts(category = 'all') {
    console.log("Fetching local products for category:", category);
    const loadingText = document.getElementById('shopLoadingText');
    if (loadingText) loadingText.style.display = 'block';

    try {
        const res = await fetch(`${API_BASE}/products?category=${category}`);
        if (!res.ok) throw new Error(`HTTP error! status: ${res.status}`);

        const data = await res.json();
        console.log("Local products received:", data.length);

        // Map local DB products
        liveProducts = data.map(p => {
            return {
                id: p.id,
                title: p.title,
                thumbnail: p.thumbnail || 'https://via.placeholder.com/150/1E1E1E/FF5A00?text=Grocery',
                brand: p.brand || 'Local Farm',
                price: p.price || 5.99,
                calories: p.calories || 0,
                allowedAmounts: p.allowedAmounts || '1-30',
                unitId: p.unitId || 1,
                unitName: p.unitName || 'count'
            };
        });

        if (loadingText) loadingText.style.display = 'none';
        renderShop(liveProducts);
    } catch (err) {
        console.error("Error fetching local products:", err);
        if (loadingText) loadingText.innerText = "Failed to load local products. Ensure backend is running.";
    }
}

function renderShop(products) {
    const container = document.getElementById('shopProductsContainer');
    container.innerHTML = '';
    products.forEach(p => {
        container.innerHTML += `
            <div class="card">
                <div class="card-img-placeholder" style="background-image: url('${p.thumbnail}');"></div>
                <div class="card-content">
                    <h3>${p.title}</h3>
                    <p style="font-size: 0.8rem; color:var(--text-muted); margin-bottom: 0.5rem;"><i class="fa-solid fa-fire"></i> ${p.calories} Calories • ${p.brand || 'Local'}</p>
                    <span class="price-tag">$${p.price.toFixed(2)}</span>
                    <div style="display: flex; gap: 0.5rem; margin-top: 1rem;">
                        <button class="btn-add" style="flex: 1; padding: 0.5rem;" onclick="addToCart('${p.title.replace(/'/g, "")}', ${p.price}, document.getElementById('amount-${p.id}').value)">Add</button>
                        <select id="amount-${p.id}" style="flex: 1; margin: 0; padding: 0.5rem; background: var(--bg-main); color: var(--text-main); border: 1px solid rgba(255,255,255,0.2); border-radius: 4px;">
                            ${generateAmountOptions(p.allowedAmounts)}
                        </select>
                        <span style="display: flex; align-items: center; color: var(--text-muted); font-size: 0.9rem;">${p.unitName}</span>
                    </div>
                </div>
            </div>
        `;
    });
}

function filterShop() {
    const term = document.getElementById('shopSearchInput').value.toLowerCase();
    const filtered = liveProducts.filter(p => p.title.toLowerCase().includes(term)).slice(0, 60);
    renderShop(filtered);
}

function setCategory(cat, btnElement) {
    document.querySelectorAll('.category-btn').forEach(btn => btn.classList.remove('active'));
    if (btnElement) btnElement.classList.add('active');
    document.getElementById('shopSearchInput').value = ''; // clear search
    fetchLiveProducts(cat);
}

function toggleCustomBudget() {
    const budgetSelect = document.getElementById('aiFilterBudget');
    const customContainer = document.getElementById('customBudgetContainer');
    if (budgetSelect.value === 'custom') {
        customContainer.style.display = 'block';
    } else {
        customContainer.style.display = 'none';
    }
}

async function generateAIRecipe() {
    const style = document.getElementById('aiFilterStyle').value;
    const budget = document.getElementById('aiFilterBudget').value;
    const customBudget = document.getElementById('aiCustomBudget').value;
    const taste = document.getElementById('aiFilterTaste').value;

    let budgetText = budget === 'pantry' ? "Only from pantry" :
        budget === 'custom' ? `$${customBudget || 0} extra` :
            `$${budget} extra`;

    // Mock fetching allergies if logged in
    let allergyText = "";
    if (currentUser && currentUser.allergies && currentUser.allergies.length > 0) {
        allergyText = "Allergies to avoid: " + currentUser.allergies.map(a => a.title || a.name || "Unknown").join(", ");
    } else {
        allergyText = "No known allergies.";
    }

    const outputContainer = document.getElementById('aiRecipeOutputContainer');
    const loading = document.getElementById('aiLoading');
    const content = document.getElementById('aiRecipeContent');

    outputContainer.style.display = 'block';
    loading.style.display = 'block';
    content.style.display = 'none';

    try {
        let area = 'American'; // fallback
        const styleMap = {
            'italian': 'Italian',
            'mexican': 'Mexican',
            'asian': 'Chinese',
            'american': 'American',
            'indian': 'Indian',
            'mediterranean': 'Greek'
        };
        
        if (styleMap[style]) {
            area = styleMap[style];
        } else if (style === 'any') {
            area = 'Canadian';
        }

        const apiUrl = `https://www.themealdb.com/api/json/v1/1/filter.php?a=${area}`;
        const res = await fetch(apiUrl);
        const data = await res.json();
        
        let meals = data.meals || [];
        if (meals.length === 0) {
             throw new Error("No recipes found");
        }
        
        // Randomly select up to 3 meals
        const shuffled = meals.sort(() => 0.5 - Math.random());
        const selectedMeals = shuffled.slice(0, 3);
        
        // Fetch full details for these 3 meals
        const fullMeals = [];
        for (const meal of selectedMeals) {
            const detailRes = await fetch(`https://www.themealdb.com/api/json/v1/1/lookup.php?i=${meal.idMeal}`);
            const detailData = await detailRes.json();
            if (detailData.meals && detailData.meals.length > 0) {
                fullMeals.push(detailData.meals[0]);
            }
        }

        const pantryNames = pantryItems.map(i => i.name).join(', ') || "No pantry items available";
        const promptText = `<strong>Real Recipe Search:</strong> "Searching TheMealDB for ${style} recipes. Considering ingredients: ${pantryNames}. Budget: ${budgetText}. ${allergyText}"`;

        let optionsHTML = '';
        fullMeals.forEach((meal, idx) => {
            const num = idx + 1;
            
            // Extract ingredients and measures, cross-reference with pantry and shop
            let pantryListHTML = '';
            let shopListHTML = '';
            let unavailableListHTML = '';
            let shopProductsToCart = [];
            let totalShopPrice = 0;

            for (let i = 1; i <= 20; i++) {
                const ingredient = meal[`strIngredient${i}`];
                const measure = meal[`strMeasure${i}`];
                if (ingredient && ingredient.trim() !== '') {
                    const ingName = ingredient.trim().toLowerCase();
                    
                    // 1. Pantry Check
                    const hasInPantry = pantryItems.some(p => {
                        const pName = p.name ? p.name.toLowerCase() : "";
                        return pName.includes(ingName) || ingName.includes(pName);
                    });

                    if (hasInPantry) {
                        pantryListHTML += `<li>✅ ${measure} ${ingredient}</li>`;
                        continue;
                    }

                    // 2. Shop Check
                    const shopProduct = liveProducts.find(p => {
                        const pTitle = p.title.toLowerCase();
                        return pTitle.includes(ingName) || ingName.includes(pTitle);
                    });

                    if (shopProduct) {
                        shopListHTML += `<li>🛒 ${measure} ${ingredient} <span style="color:var(--secondary-color); font-size:0.8rem;">($${shopProduct.price.toFixed(2)})</span></li>`;
                        shopProductsToCart.push({ name: shopProduct.title, price: shopProduct.price });
                        totalShopPrice += shopProduct.price;
                    } else {
                        // 3. Unavailable
                        unavailableListHTML += `<li>❌ ${measure} ${ingredient} <span style="color:#ff6b6b; font-size:0.8rem;">(Not in shop)</span></li>`;
                    }
                }
            }

            let combinedIngredientsHTML = '';
            if (pantryListHTML) {
                combinedIngredientsHTML += `
                    <h5 style="margin-top:0.5rem; color:var(--text-main); font-size:0.9rem;">In Your Pantry</h5>
                    <ul style="margin-left: 1.5rem; margin-bottom: 0.5rem; color: var(--text-muted); font-size: 0.9rem;">${pantryListHTML}</ul>
                `;
            }
            if (shopListHTML) {
                combinedIngredientsHTML += `
                    <h5 style="margin-top:0.5rem; color:var(--text-main); font-size:0.9rem;">Missing (Available in Shop)</h5>
                    <ul style="margin-left: 1.5rem; margin-bottom: 0.5rem; color: var(--text-muted); font-size: 0.9rem;">${shopListHTML}</ul>
                `;
            }
            if (unavailableListHTML) {
                combinedIngredientsHTML += `
                    <h5 style="margin-top:0.5rem; color:var(--text-main); font-size:0.9rem;">Missing (Not in Shop)</h5>
                    <ul style="margin-left: 1.5rem; margin-bottom: 0.5rem; color: var(--text-muted); font-size: 0.9rem;">${unavailableListHTML}</ul>
                `;
            }

            const cartPayload = encodeURIComponent(JSON.stringify(shopProductsToCart));

            optionsHTML += `
            <div class="ai-recipe-option" style="background: rgba(0,0,0,0.2); border: 1px solid rgba(255,255,255,0.1); border-radius: 8px; padding: 1.5rem; margin-bottom: 2rem;">
                <div style="display: flex; gap: 1rem; align-items: flex-start; margin-bottom: 1rem; flex-wrap: wrap;">
                    <img src="${meal.strMealThumb}" alt="${meal.strMeal}" style="width: 120px; height: 120px; object-fit: cover; border-radius: 8px;">
                    <div style="flex: 1; min-width: 200px;">
                        <h3 style="color: var(--primary-color); margin-bottom: 0.5rem;"><i class="fa-solid fa-utensils"></i> Option ${num}: ${meal.strMeal}</h3>
                        <p style="color: var(--text-muted); font-size: 0.9rem;">Category: ${meal.strCategory} | Area: ${meal.strArea}</p>
                        <p style="color: var(--text-muted); font-size: 0.9rem; margin-top: 0.5rem;">Estimated extra cost: $${totalShopPrice.toFixed(2)}</p>
                    </div>
                </div>

                <div style="margin-bottom: 1rem;">
                    <h4 style="margin-bottom: 0.5rem; color: var(--text-main);">Ingredients</h4>
                    ${combinedIngredientsHTML}

                    <h4 style="margin-top: 1rem; margin-bottom: 0.5rem; color: var(--text-main);">Instructions</h4>
                    <p style="color: var(--text-muted); font-size: 0.95rem; white-space: pre-wrap; line-height: 1.6;">${meal.strInstructions}</p>
                </div>
                
                <button class="btn btn-outline" style="width: 100%; margin-top: 1rem;" onclick="addMultipleToCart('${cartPayload}', this)" ${shopProductsToCart.length === 0 ? 'disabled style="opacity: 0.5;"' : ''}>
                    <i class="fa-solid fa-cart-shopping"></i> Add missing ingredients to cart ($${totalShopPrice.toFixed(2)})
                </button>
            </div>
            `;
        });

        const mockRecipe = `
            <div style="margin-bottom: 2rem;">
                <h3 style="color: var(--primary-color); margin-bottom: 0.5rem;"><i class="fa-solid fa-star"></i> Real Recipe Results</h3>
                <div style="background: rgba(255, 90, 0, 0.1); border-left: 4px solid var(--primary-color); padding: 1rem; margin-bottom: 1.5rem; border-radius: 4px; font-size: 0.9rem;">
                    ${promptText}
                </div>
            </div>
            ${optionsHTML}
        `;

        loading.style.display = 'none';
        content.innerHTML = mockRecipe;
        content.style.display = 'block';
        content.scrollIntoView({ behavior: 'smooth', block: 'start' });

    } catch (error) {
        console.error("Error fetching recipes:", error);
        loading.style.display = 'none';
        content.innerHTML = `<div style="color: red; padding: 1rem;">Failed to fetch recipes. Please try again.</div>`;
        content.style.display = 'block';
    }
}

// ---------------- RENDER BUNDLES ----------------
async function loadBundles() {
    const bc = document.getElementById('bundlesContainer');
    if (!bc) return;
    
    // Bundles will be fetched from API in the future
    bc.innerHTML = '<p style="color: var(--text-muted); text-align: center;">Bundles are currently unavailable.</p>';
}

setTimeout(loadBundles, 200);

// ---------------- FOOTER ROUTING ----------------
function showFooterInfo(pageTitle) {
    switchTab('info');
    document.getElementById('infoHeroTitle').innerText = pageTitle;
    document.getElementById('infoContentTitle').innerText = `${ pageTitle } Documentation`;
    document.getElementById('infoContentText').innerHTML = `
            < p style = "margin-bottom: 1rem;" > This is the official simulated landing page for the < strong > ${ pageTitle }</strong > section.</p >
        <p style="margin-bottom: 1rem;">As a premium digital grocery platform, SmartChef is deeply committed to transparency, quality, and exceptional service. The details of our policies regarding ${pageTitle.toLowerCase()} are currently being updated by our legal and operational teams to ensure the highest standards compliance.</p>
        <p>If you have any immediate concerns or require full textual data for ${pageTitle}, please contact our SmartChef support channel directly.</p>
        `;
    window.scrollTo({ top: 0, behavior: 'smooth' });
}
