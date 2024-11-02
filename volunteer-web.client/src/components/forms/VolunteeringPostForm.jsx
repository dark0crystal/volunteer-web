import { useState } from 'react';

const initialFormState = {
    title: '',
    orgName: '',
    category: '',
    description: '',
    location: '',
    startDate: '',
    endDate: '',
    numOfDays: '',
    postAdminEmail: 'user@gmail.com' // Default email for the admin
};

const VolunteeringPostForm = () => {
    const [formData, setFormData] = useState(initialFormState);
    const [formErrors, setFormErrors] = useState({});
    const [isLoading, setIsLoading] = useState(false);
    const [submittedEvents, setSubmittedEvents] = useState([]);

    // Map categories to their corresponding enum values (0, 1, 2, ...)
    const categories = [
        { label: 'Education', value: 0 },
        { label: 'Environment', value: 1 },
        { label: 'Health', value: 2 },
        { label: 'Community', value: 3 },
        { label: 'Animal Welfare', value: 4 },
    ];

    const handleInputChange = (e) => {
        const { name, value } = e.target;

        setFormData((prevState) => ({
            ...prevState,
            [name]: value,
        }));

        let errors = { ...formErrors };

        // Validation rules for fields
        if (name === 'numOfDays' && (!Number.isInteger(+value) || +value <= 0)) {
            errors.numOfDays = 'Number of days must be a positive integer.';
        } else if (name === 'orgName' && value.length < 3) {
            errors.orgName = 'Organization name must be at least 3 characters long.';
        } else if (name === 'category' && !value) {
            errors.category = 'Please select a category.';
        } else if (name === 'title' && value.length < 3) {
            errors.title = 'Title name must be at least 3 characters long.';
        } else if (name === 'description' && value.length < 10) {
            errors.description = 'Description must be at least 10 characters long.';
        } else if (name === 'location' && value.length < 3) {
            errors.location = 'Location must be at least 3 characters long.';
        } else if (name === 'startDate' && !Date.parse(value)) {
            errors.startDate = 'Please enter a valid start date.';
        } else if (name === 'endDate' && !Date.parse(value)) {
            errors.endDate = 'Please enter a valid end date.';
        } else {
            errors[name] = ''; // Clear error if field is valid
        }

        setFormErrors(errors);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setIsLoading(true);
        setFormErrors({}); // Reset errors

        const isFormValid =
            Object.values(formErrors).every((error) => error === '') &&
            Object.values(formData).every((field) => field.trim() !== '');

        console.log('Form Data before submission:', formData);

        if (isFormValid) {
            // Convert category string to integer
            const categoryInt = parseInt(formData.category, 10);
            const dataToSubmit = { ...formData, category: categoryInt };

            try {
                const response = await fetch('https://localhost:7149/Posts/addpost', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(dataToSubmit),
                });

                if (!response.ok) {
                    const errorData = await response.json();
                    console.error('Error response from server:', errorData);
                    setFormErrors(errorData.errors || {});
                } else {
                    const newEvent = await response.json();
                    setSubmittedEvents((prevEvents) => [...prevEvents, newEvent]);

                    // Reset the form
                    setFormData(initialFormState);
                    setFormErrors({});
                }
            } catch (error) {
                console.error('Fetch error:', error);
            }
        } else {
            console.log('Form contains validation errors:', formErrors);
        }

        setIsLoading(false);
    };

    return (
        <div className="container mt-5">
            <h2 className="mb-4">Event Form</h2>
            <form onSubmit={handleSubmit} className="bg-light p-4 border rounded">
                {/* Title Field */}
                <div className="mb-3">
                    <label htmlFor="title" className="form-label">Title:</label>
                    <input
                        type="text"
                        name="title"
                        className={`form-control ${formErrors.title ? 'is-invalid' : ''}`}
                        value={formData.title}
                        onChange={handleInputChange}
                    />
                    {formErrors.title && <div className="invalid-feedback">{formErrors.title}</div>}
                </div>

                {/* Organization Name Field */}
                <div className="mb-3">
                    <label htmlFor="orgName" className="form-label">Organization Name:</label>
                    <input
                        type="text"
                        name="orgName"
                        className={`form-control ${formErrors.orgName ? 'is-invalid' : ''}`}
                        value={formData.orgName}
                        onChange={handleInputChange}
                    />
                    {formErrors.orgName && <div className="invalid-feedback">{formErrors.orgName}</div>}
                </div>

                {/* Category Field */}
                <div className="mb-3">
                    <label htmlFor="category" className="form-label">Category:</label>
                    <select
                        name="category"
                        className={`form-select ${formErrors.category ? 'is-invalid' : ''}`}
                        value={formData.category}
                        onChange={handleInputChange}
                    >
                        <option value="">Select a category</option>
                        {categories.map((category) => (
                            <option key={category.value} value={category.value}>
                                {category.label}
                            </option>
                        ))}
                    </select>
                    {formErrors.category && <div className="invalid-feedback">{formErrors.category}</div>}
                </div>

                {/* Description Field */}
                <div className="mb-3">
                    <label htmlFor="description" className="form-label">Description:</label>
                    <textarea
                        name="description"
                        className={`form-control ${formErrors.description ? 'is-invalid' : ''}`}
                        value={formData.description}
                        onChange={handleInputChange}
                    />
                    {formErrors.description && <div className="invalid-feedback">{formErrors.description}</div>}
                </div>

                {/* Location Field */}
                <div className="mb-3">
                    <label htmlFor="location" className="form-label">Location:</label>
                    <input
                        type="text"
                        name="location"
                        className={`form-control ${formErrors.location ? 'is-invalid' : ''}`}
                        value={formData.location}
                        onChange={handleInputChange}
                    />
                    {formErrors.location && <div className="invalid-feedback">{formErrors.location}</div>}
                </div>

                {/* Start Date Field */}
                <div className="mb-3">
                    <label htmlFor="startDate" className="form-label">Start Date:</label>
                    <input
                        type="date"
                        name="startDate"
                        className={`form-control ${formErrors.startDate ? 'is-invalid' : ''}`}
                        value={formData.startDate}
                        onChange={handleInputChange}
                    />
                    {formErrors.startDate && <div className="invalid-feedback">{formErrors.startDate}</div>}
                </div>

                {/* End Date Field */}
                <div className="mb-3">
                    <label htmlFor="endDate" className="form-label">End Date:</label>
                    <input
                        type="date"
                        name="endDate"
                        className={`form-control ${formErrors.endDate ? 'is-invalid' : ''}`}
                        value={formData.endDate}
                        onChange={handleInputChange}
                    />
                    {formErrors.endDate && <div className="invalid-feedback">{formErrors.endDate}</div>}
                </div>

                {/* Number of Days Field */}
                <div className="mb-3">
                    <label htmlFor="numOfDays" className="form-label">Number of Days:</label>
                    <input
                        type="number"
                        name="numOfDays"
                        className={`form-control ${formErrors.numOfDays ? 'is-invalid' : ''}`}
                        value={formData.numOfDays}
                        onChange={handleInputChange}
                    />
                    {formErrors.numOfDays && <div className="invalid-feedback">{formErrors.numOfDays}</div>}
                </div>

                {/* Post Admin Email Field */}
                <div className="mb-3">
                    <label htmlFor="postAdminEmail" className="form-label">Admin Email:</label>
                    <input
                        type="email"
                        name="postAdminEmail"
                        className={`form-control ${formErrors.postAdminEmail ? 'is-invalid' : ''}`}
                        value={formData.postAdminEmail}
                        onChange={handleInputChange}
                    />
                    {formErrors.postAdminEmail && <div className="invalid-feedback">{formErrors.postAdminEmail}</div>}
                </div>

                {/* Submit Button */}
                <button type="submit" className="btn btn-primary" disabled={isLoading}>
                    {isLoading ? 'Submitting...' : 'Submit'}
                </button>
            </form>

            {/* Display submitted events */}
            {submittedEvents.length > 0 && (
                <div className="mt-4">
                    <h2>Submitted Events</h2>
                    <ul className="list-group">
                        {submittedEvents.map((event, index) => (
                            <li key={index} className="list-group-item">
                                {event.title} - {event.orgName}
                            </li>
                        ))}
                    </ul>
                </div>
            )}
        </div>
    );
};

export default VolunteeringPostForm;
