import { useState } from 'react';
import '../index.css';

const BoletaCard = ({ title, children }) => {
    const [isOpen, setIsOpen] = useState(false);

    return (
        <div className="border rounded-lg overflow-hidden shadow-md mb-4 bg-gray-800 border-gray-700">
            <button
                className="w-full p-4 text-left font-medium bg-gray-700 hover:bg-gray-600 transition-colors duration-200 flex justify-between items-center text-white"
                onClick={() => setIsOpen(!isOpen)}
            >
                <span className="text-lg">{title}</span>
                <span className={`transform transition-transform duration-200 ${isOpen ? 'rotate-180' : ''}`}>
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 9l-7 7-7-7" />
                    </svg>
                </span>
            </button>

            <div className={`transition-all duration-200 overflow-hidden ${isOpen ? 'max-h-[5000px]' : 'max-h-0'}`}>
                <div className="p-4 bg-gray-800">
                    {children}
                </div>
            </div>
        </div>
    );
};

export default BoletaCard;