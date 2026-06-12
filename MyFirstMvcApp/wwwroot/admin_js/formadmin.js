// File input rename utility
    function updateFileName(input) {
      const label = document.getElementById('fileLabel');
      if (input.files && input.files.length > 0) {
        label.textContent = `Selected: ${input.files[0].name}`;
      } else {
        label.innerHTML = 'Drop file here or <strong>browse</strong>';
      }
    }

