import { useState } from 'react';
import DisplayComplaints from "../components/card/Complaints";
import { useParams } from "react-router-dom";

export default function Complaints() {
    const { postId } = useParams();  // Capture the postId from the URL
    const [formData, setFormData] = useState({
        PostId: postId,
        ComplaintType: '',
        ComplaintText: '',
    });

    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');

    // Handle form input changes
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrorMessage('');
        setSuccessMessage('');

        try {
            const response = await fetch('https://localhost:7149/Complaints/SubmitComplaint', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(formData),
            });

            if (response.ok) {
                const data = await response.json();
                setSuccessMessage(data.message);
                setFormData({ ComplaintType: '', ComplaintText: '' }); // Clear form
            } else {
                const error = await response.json();
                setErrorMessage(error.message || 'An error occurred.');
            }
        } catch (error) {
            setErrorMessage('Failed to submit complaint. Please try again later.', error);
        }
    };

    return (
        <div className="container mt-5">
            
            {errorMessage && <div className="alert alert-danger">{errorMessage}</div>}
            {successMessage && <div className="alert alert-success">{successMessage}</div>}
            <form onSubmit={handleSubmit}>
                {/* Complaint form fields */}
                <div className="form-group mt-3">
                    <label>Type of Complaint</label>
                    <div className="form-check">
                        <input
                            type="radio"
                            id="spam"
                            name="ComplaintType"
                            value="Spam"
                            checked={formData.ComplaintType === 'Spam'}
                            onChange={handleChange}
                            className="form-check-input"
                        />
                        <label className="form-check-label" htmlFor="spam">Spam</label>
                    </div>
                    <div className="form-check">
                        <input
                            type="radio"
                            id="inappropriate"
                            name="ComplaintType"
                            value="Inappropriate Content"
                            checked={formData.ComplaintType === 'Inappropriate Content'}
                            onChange={handleChange}
                            className="form-check-input"
                        />
                        <label className="form-check-label" htmlFor="inappropriate">Inappropriate Content</label>
                    </div>
                    <div className="form-check">
                        <input
                            type="radio"
                            id="misleading"
                            name="ComplaintType"
                            value="Misleading Information"
                            checked={formData.ComplaintType === 'Misleading Information'}
                            onChange={handleChange}
                            className="form-check-input"
                        />
                        <label className="form-check-label" htmlFor="misleading">Misleading Information</label>
                    </div>
                </div>

                <div className="form-group mt-3">
                    <label htmlFor="ComplaintText">Your Complaint</label>
                    <textarea
                        id="ComplaintText"
                        name="ComplaintText"
                        value={formData.ComplaintText}
                        onChange={handleChange}
                        className="form-control"
                        rows="5"
                        placeholder="Write your complaint here"
                        required
                    ></textarea>
                </div>

                <button type="submit" className="btn btn-primary mt-3">Submit Complaint</button>
            </form>
            <div>

                <DisplayComplaints />
            </div>
        </div>
    );
}
