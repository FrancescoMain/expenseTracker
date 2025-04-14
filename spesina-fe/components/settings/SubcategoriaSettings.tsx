"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";

interface Category {
  id: number;
  name: string;
}

interface Subcategory {
  id: number;
  name: string;
  categoryId: number;
}

export default function SubcategoriaSettings() {
  const { token } = useAuth();
  const [categories, setCategories] = useState<Category[]>([]);
  const [subcategories, setSubcategories] = useState<Subcategory[]>([]);
  const [selectedCategory, setSelectedCategory] = useState<string>("");
  const [newSubcat, setNewSubcat] = useState("");

  const fetchCategories = async () => {
    const res = await apiFetch("http://localhost:5299/api/Category", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setCategories(data.data);
  };

  const fetchSubcategories = async () => {
    const res = await apiFetch("http://localhost:5299/api/Subcategory", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setSubcategories(data.data);
  };

  const handleAddSubcategory = async () => {
    if (!newSubcat || !selectedCategory) return;
    await apiFetch("http://localhost:5299/api/Subcategory", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        name: newSubcat,
        categoryId: Number(selectedCategory),
      }),
    });
    setNewSubcat("");
    fetchSubcategories();
  };

  const handleDeleteSubcategory = async (id: number) => {
    await apiFetch(`http://localhost:5299/api/Subcategory/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });
    fetchSubcategories();
  };

  useEffect(() => {
    if (!token) return;
    fetchCategories();
    fetchSubcategories();
  }, [token]);

  return (
    <div className="bc-ter rounded-xl shadow p-6 mt-6 col-sec">
      <h2 className="text-xl  font-semibold mb-4 flex items-center gap-2">
        📁 Sottocategorie
      </h2>

      <div className="flex gap-2 mb-4">
        <select
          className="border rounded p-2 flex-1"
          value={selectedCategory}
          onChange={(e) => setSelectedCategory(e.target.value)}
        >
          <option value="">Seleziona categoria</option>
          {categories.map((c) => (
            <option key={c.id} value={c.id}>
              {c.name}
            </option>
          ))}
        </select>

        <input
          className="border rounded p-2 flex-1"
          placeholder="Nome sottocategoria"
          value={newSubcat}
          onChange={(e) => setNewSubcat(e.target.value)}
        />
        <button
          className="bg-green-600 hover:bg-green-700 text-white px-4 rounded"
          onClick={handleAddSubcategory}
        >
          Aggiungi
        </button>
      </div>

      {selectedCategory && (
        <ul className="space-y-2">
          {subcategories
            .filter((s) => s.categoryId === Number(selectedCategory))
            .map((sub) => (
              <li
                key={sub.id}
                className="flex justify-between items-center  px-4 py-2 rounded"
              >
                <span>{sub.name}</span>
                <button
                  className="text-sm text-red-500 hover:underline"
                  onClick={() => handleDeleteSubcategory(sub.id)}
                >
                  Elimina
                </button>
              </li>
            ))}
        </ul>
      )}
    </div>
  );
}
