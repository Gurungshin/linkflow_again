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