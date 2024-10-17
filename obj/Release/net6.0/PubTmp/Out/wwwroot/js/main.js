document.addEventListener('DOMContentLoaded', function () {
    const addButton = document.getElementById('add-component-btn');
    const container = document.getElementById('components-container');
    let componentCount = 0;

    addButton.addEventListener('click', function () {
        componentCount++;
        const newDiv = document.createElement('div');
        newDiv.classList.add('input-div', 'one');
        newDiv.innerHTML = `
            <div class="remove-btn" onclick="this.parentElement.remove()">X</div>
            <br>
            <div class="i">
                <i class="fas fa-user"></i>
            </div>
            <div class="div">
                <input type="text" name="ComponentDto[${componentCount}].Name" placeholder="Nom du composant" class="input">
            </div>
            <div class="i">
                <i class="fas fa-user"></i>
            </div>
            <div class="div">
                <input type="number" name="ComponentDto[${componentCount}].Quantity" min="0" placeholder="Quantité" class="input">
            </div>
            <div class="i">
                <i class="fas fa-user"></i>
            </div>
            <div class="div">
                <input type="number" name="ComponentDto[${componentCount}].Price" min="0" placeholder="Prix" class="input">
            </div>
        `;
        container.appendChild(newDiv);
    });
});
