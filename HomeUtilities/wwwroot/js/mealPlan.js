$(document).ready(function () {
    // Set up the tag pickers for the search page
    $('.js-included-tags').select2();
    $('.js-excluded-tags').select2();

    // Set up the ajax handlers for the saved recipe functionality
    // TODO: Maybe break this out into it's own js file
    const savedRecipeButtons = document.querySelectorAll('.saved-recipe-button');

    savedRecipeButtons.forEach(button => {
        button.addEventListener('click', async function () {
            const recipeId = this.dataset.recipeId;
            const heartIcon = this.querySelector('.heart-icon');
            const isCurrentlyFavorite = heartIcon.dataset.isSaved === 'true';

            let url = '';
            if (isCurrentlyFavorite) {
                url = `/MealPlan/RemoveSavedRecipe?recipeId=${recipeId}`;
            } else {
                url = `/MealPlan/SaveRecipe?recipeId=${recipeId}`;
            }

            try {
                const response = await fetch(url, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'X-Requested-With': 'XMLHttpRequest'
                    }
                });

                if (response.ok) {
                    const success = await response.json();
                    if (success) {
                        heartIcon.dataset.isFavorite = !isCurrentlyFavorite;
                        heartIcon.querySelector('path[fill]').setAttribute('fill', !isCurrentlyFavorite ? 'red' : 'none');
                    } else {
                        // Handle the case where the backend operation failed (optional)
                        console.error('Failed to update favorite status on the server.');
                    }
                } else {
                    console.error('Error during AJAX request:', response.status);
                }
            } catch (error) {
                console.error('AJAX request failed:', error);
            }
        });
    });
});