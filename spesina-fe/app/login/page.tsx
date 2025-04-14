"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";

export default function LoginPage() {
  const router = useRouter();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const { login } = useAuth();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");

    const res = await apiFetch("http://localhost:5299/api/Auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password }),
    });

    const data = await res.json();

    if (!res.ok) {
      setError(data.message || "Errore durante il login");
      return;
    }

    // Usiamo il context per gestire tutto
    const user = {
      email: data.data.email,
      role: data.data.role,
    };
    login(user, data.data.token);
    router.push("/dashboard");
  };
  return (
    <main className="min-h-screen flex items-center justify-center bc-prim">
      <form
        onSubmit={handleLogin}
        className="bg-white p-8 rounded shadow-md w-full max-w-md space-y-6 bc-ter"
      >
        <h1 className="text-2xl font-bold text-center col-qua2">Login</h1>

        {error && <p className="text-red-500 text-sm text-center">{error}</p>}

        <input
          type="email"
          placeholder="Email"
          className="w-full border px-4 py-2 rounded col-qua2"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
        <input
          type="password"
          placeholder="Password"
          className="w-full border px-4 py-2 rounded col-qua2"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />

        <button
          type="submit"
          className="w-full bc-qua text-white py-2 rounded transition submit col-sec"
        >
          Accedi
        </button>
      </form>
    </main>
  );
}
