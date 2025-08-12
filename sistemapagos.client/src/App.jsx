// src/App.jsx
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import LoginView from './Views/LoginView.jsx'
import AdminView from './Views/AdminView.jsx'
import EmpleadoView from './Views/EmpleadoView.jsx';

// Componente de ruta protegida
const ProtectedRoute = ({ children }) => {
    const user = JSON.parse(localStorage.getItem('user'));
    return user ? children : <Navigate to="/" replace />;
};

// Componente principal App
export const App = () => {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<LoginView />} />
                <Route
                    path="/admin"
                    element={
                        <ProtectedRoute>
                            <AdminView />
                        </ProtectedRoute>
                    }
                />
                <Route
                    path="/empleado"
                    element={
                        <ProtectedRoute>
                            <EmpleadoView />
                        </ProtectedRoute>
                    }
                />
            </Routes>
        </BrowserRouter>
    );
};