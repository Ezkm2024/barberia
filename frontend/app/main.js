// This is a very small shim to bootstrap the Angular-like app without full Angular CLI.
// For a production-ready Angular app, use the Angular CLI to scaffold and build the project.
(function(){
  // Very small components rendering using DOM directly is implemented in index.html
  // This file not implementing actual Angular. It's a placeholder.
  document.addEventListener('DOMContentLoaded', function(){
    var root = document.querySelector('app-root');
    if (!root) return;
    root.innerHTML = '<h1>Barbería - Frontend (placeholder)</h1><p>Use Angular CLI to generate a full project. See README.</p>';
  });
})();
