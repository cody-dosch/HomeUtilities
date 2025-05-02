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
            const isCurrentlySaved = heartIcon.dataset.isSaved === 'true';

            let url = '';
            if (isCurrentlySaved) {
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
                        heartIcon.dataset.isSaved = !isCurrentlySaved;
                        heartIcon.querySelector('path[fill]').setAttribute('fill', !isCurrentlySaved ? 'red' : 'none');
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

    // Set up the ajax handlers for the select recipe functionality
    // TODO: Maybe break this out into it's own js file
    const selectRecipeButtons = document.querySelectorAll('.selected-recipe-button');

    selectRecipeButtons.forEach(button => {
        button.addEventListener('click', async function () {
            const recipeId = this.dataset.recipeId;
            const isCurrentlySelected = this.dataset.isSelected === 'true';

            let url = '';
            if (!isCurrentlySelected) {
                url = `/MealPlan/SelectRecipe?recipeId=${recipeId}`;
            } else {
                url = `/MealPlan/UnselectRecipe?recipeId=${recipeId}`;
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
                        this.dataset.isSelected = !isCurrentlySelected;

                        // Update button text - if it wasn't selected now it is, and vice versa.
                        if (!isCurrentlySelected) {
                            this.innerText = 'Remove from Plan';
                        }
                        else {
                            this.innerText = 'Add to Plan';
                        }
                    } else {
                        // Handle the case where the backend operation failed
                        console.error('Failed to update selected status on the server.');
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