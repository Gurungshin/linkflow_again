  // Sidebar dropdowns
    function toggleDropdown(id, el) {
      const isOpen = el.classList.contains('open');
      document.querySelectorAll('.nav-dropdown').forEach(d => d.classList.remove('open'));
      document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('open'));
      if (!isOpen) { document.getElementById(id).classList.add('open'); el.classList.add('open'); }
    }

    // Mobile drawer
    const sidebar = document.getElementById('sidebar');
    document.getElementById('menuBtn')?.addEventListener('click', e => { sidebar.classList.toggle('open'); e.stopPropagation(); });
    document.addEventListener('click', e => { if (!sidebar.contains(e.target)) sidebar.classList.remove('open'); });

    // Table: search + pagination
    const PER_PAGE = 10;
    let page = 1, rows = [], filtered = [];

    function render() {
      const total = filtered.length;
      const start = (page - 1) * PER_PAGE;
      const end   = Math.min(start + PER_PAGE, total);

      rows.forEach(r => r.hidden = true);
      filtered.slice(start, end).forEach(r => r.hidden = false);

      document.getElementById('noResults').style.display = total ? 'none' : 'block';
      document.getElementById('paginationInfo').innerHTML =
        total ? `Showing <strong>${start + 1}–${end}</strong> of ${total}` : '';

      const btns = document.getElementById('paginationBtns');
      const pages = Math.ceil(total / PER_PAGE);
      btns.innerHTML = '';
      const mk = (label, pg, disabled, active) => {
        const b = Object.assign(document.createElement('button'), { textContent: label, disabled });
        if (active) b.classList.add('active');
        b.onclick = () => { page = pg; render(); };
        btns.appendChild(b);
      };
      mk('← Prev', page - 1, page === 1, false);
      for (let i = 1; i <= pages; i++) mk(i, i, false, i === page);
      mk('Next →', page + 1, page === pages || pages === 0, false);
    }

    document.getElementById('tableSearch').addEventListener('input', function () {
      const q = this.value.toLowerCase();
      filtered = rows.filter(r => r.textContent.toLowerCase().includes(q));
      page = 1;
      render();
    });

    rows = filtered = [...document.querySelectorAll('.table-row')];
    render();


     // File input rename utility
    function updateFileName(input) {
      const label = document.getElementById('fileLabel');
      if (input.files && input.files.length > 0) {
        label.textContent = `Selected: ${input.files[0].name}`;
      } else {
        label.innerHTML = 'Drop file here or <strong>browse</strong>';
      }
    }

    // Initialize CKEditor 5 on the textarea element
    ClassicEditor
      .create(document.querySelector('#editor'), {
        toolbar: [ 'heading', '|', 'bold', 'italic', 'link', 'bulletedList', 'numberedList', 'blockQuote', 'undo', 'redo' ]
      })
      .catch(error => {
        console.error(error);
      });