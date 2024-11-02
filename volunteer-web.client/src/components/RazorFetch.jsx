import { useEffect, useState } from 'react';

export default function RazorView() {
    const [htmlContent, setHtmlContent] = useState('');

    useEffect(() => {
        async function fetchRazorView() {
            try {
                const response = await fetch('https://localhost:7149/Home/index'); // Adjust the URL as necessary
                if (!response.ok) {
                    throw new Error('Failed to fetch the Razor view');
                }
                const text = await response.text();
                setHtmlContent(text);
            } catch (error) {
                console.error('Error fetching Razor view:', error);
            }
        }

        fetchRazorView();
    }, []);

    return (
        <div dangerouslySetInnerHTML={{ __html: htmlContent }} />
    );
}
