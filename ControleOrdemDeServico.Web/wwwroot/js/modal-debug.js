// Teste Bootstrap Modal
console.log('Bootstrap carregado:', typeof bootstrap !== 'undefined');
console.log('Bootstrap Modal disponível:', typeof bootstrap?.Modal !== 'undefined');

// Adiciona listener para debug de modals
document.addEventListener('DOMContentLoaded', function() {
    const modalEl = document.getElementById('modal-formulario');
    if (modalEl) {
        console.log('Modal encontrada no DOM:', modalEl);
        
        modalEl.addEventListener('show.bs.modal', function () {
            console.log('Modal está sendo aberta...');
        });
        
        modalEl.addEventListener('shown.bs.modal', function () {
            console.log('Modal foi aberta!');
        });
        
        modalEl.addEventListener('hide.bs.modal', function () {
            console.log('Modal está sendo fechada...');
        });
    } else {
        console.error('Modal #modal-formulario NÃO encontrada no DOM!');
    }
    
    // Testa o botão
    const btnModal = document.querySelector('[data-bs-target="#modal-formulario"]');
    if (btnModal) {
        console.log('Botão encontrado:', btnModal);
    } else {
        console.error('Botão com data-bs-target NÃO encontrado!');
    }
});
