// components/settings/CategoriaSettings.tsx
"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";

interface Categoria {
  id: number;
  name: string;
}

export default function CategoriaSettings() {
  const { token } = useAuth();
  const [categorie, setCategorie] = useState<Categoria[]>([]);
  const [name, setName] = useState("");

  const fetchCategorie = async () => {
    const res = await apiFetch("http://localhost:5299/api/Category", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setCategorie(data.data);
  };

  const createCategoria = async () => {
    if (!name.trim()) return;
    await apiFetch("http://localhost:5299/api/Category", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ name }),
    });
    setName("");
    fetchCategorie();
  };

  const deleteCategoria = async (id: number) => {
    await apiFetch(`http://localhost:5299/api/Category/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });
    fetchCategorie();
  };

  useEffect(() => {
    if (token) fetchCategorie();
  }, [token]);

  return (
    <div>
      <div className="flex items-center gap-2 mb-4">
        <input
          className="flex-1 border p-2 rounded  col-sec placeholder-gray-400 dark:placeholder-gray-500"
          placeholder="Nome categoria"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <button
          onClick={createCategoria}
          className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded"
        >
          Aggiungi
        </button>
      </div>

      <ul className="space-y-2">
        {categorie.map((cat) => (
          <li
            key={cat.id}
            className="flex justify-between items-center px-4 py-2  rounded-md col-sec"
          >
            <span>{cat.name}</span>
            <button
              onClick={() => deleteCategoria(cat.id)}
              className="text-red-500 hover:text-red-600 text-sm"
            >
              Elimina
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
