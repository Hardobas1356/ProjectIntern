var hasUnsavedOrder = false;
var pendingOrder = [];

var container = document.getElementById('speciality-data');
var specialityId = container.dataset.specialityId;
var reorderUrl = container.dataset.reorderUrl;

window.addEventListener('beforeunload', function (e) {
    if (hasUnsavedOrder) {
        e.preventDefault();
        e.returnValue = '';
    }
});

document.addEventListener('DOMContentLoaded', function () {
    var savedState = localStorage.getItem('includeDeletedTopics');
    var checkbox = document.getElementById('toggleDeleted');
    if (savedState !== null) {
        checkbox.checked = (savedState === 'true');
    }

    document.querySelectorAll('form').forEach(form => {
        form.addEventListener('submit', () => { hasUnsavedOrder = false; });
    });

    var el = document.getElementById('sortable-topics');
    Sortable.create(el, {
        animation: 150,
        handle: '.grab-handle',
        onEnd: function () {
            var rows = el.querySelectorAll('tr[data-id]');
            pendingOrder = [];
            rows.forEach((row, index) => {
                var badge = row.querySelector('.badge.bg-secondary');
                if (badge) badge.textContent = '# ' + index;
                pendingOrder.push(row.getAttribute('data-id'));
            });
            hasUnsavedOrder = true;
            document.getElementById('save-order-btn').classList.remove('d-none');
            document.getElementById('save-order-hint').classList.remove('d-none'); 
        }
    });
});

function toggleDeletedTopics(checkbox) {
    var isChecked = checkbox.checked;
    localStorage.setItem('includeDeletedTopics', isChecked);
    var url = new URL(window.location.href);

    if (isChecked) {
        url.searchParams.set('includeDeletedTopics', 'true');
    } else {
        url.searchParams.delete('includeDeletedTopics');
    }

    window.location.href = url.toString();
}

function saveOrder() {
    if (!pendingOrder.length) return;
    var btn = document.getElementById('save-order-btn');
    btn.disabled = true;
    btn.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Saving...';

    fetch(reorderUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ specialityId: specialityId, topicIds: pendingOrder })
    })
        .then(response => {
            if (response.ok) {
                hasUnsavedOrder = false;
                btn.innerHTML = '<i class="fas fa-check"></i> Saved';
                setTimeout(() => {
                    btn.classList.add('d-none');
                    btn.disabled = false;
                    btn.innerHTML = '<i class="fas fa-save"></i> Save Order';
                    document.getElementById('save-order-hint').classList.add('d-none'); 
                }, 1500);
            } else {
                alert('Error saving order.');
                btn.disabled = false;
                btn.innerHTML = '<i class="fas fa-save"></i> Save Order';
                document.getElementById('save-order-hint').classList.add('d-none'); 
            }
        })
        .catch(() => {
            alert('Network error.');
            btn.disabled = false;
            btn.innerHTML = '<i class="fas fa-save"></i> Save Order';
        });
}