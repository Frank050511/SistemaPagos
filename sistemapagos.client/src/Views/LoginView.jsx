import "../index.css";
import LoginForm from "../Components/LoginForm.jsx";

export default function Login() {
    return (
        <>
            <div className="flex min-h-full flex-col justify-center px-6 py-12 lg:px-8">
                <div className="sm:mx-auto sm:w-full sm:max-w-sm">
                    <h2 className="mt-10 text-center text-2xl/9 font-bold tracking-tight text-white">Ingresa a tu cuenta</h2>
                </div>

                <LoginForm/>
            </div>
        </>
    )
}