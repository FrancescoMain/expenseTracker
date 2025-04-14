"use client";

import { useState, useEffect } from "react";
import { useAuth } from "@/context/AuthContext";
import { apiFetch } from "@/lib/apiFetch";

interface Props {
  onSaved: () => void;
  editingTransaction: any | null;
  clearEditing: () => void;
}

export default function NuovaTransazioneForm({
  onSaved,
  editingTransaction,
  clearEditing,
}: Props) {
  const { token } = useAuth();
  const [amount, setAmount] = useState<string>("0");

  const [description, setDescription] = useState("");
  const [date, setDate] = useState(() => new Date().toISOString().slice(0, 10));
  const [accountId, setAccountId] = useState<number | null>(null);
  const [subcategoryId, setSubcategoryId] = useState<number | null>(null);
  const [filteredSubcategories, setFilteredSubcategories] = useState<any[]>([]);
  const [selectedCategoryId, setSelectedCategoryId] = useState<number | null>(
    null
  );

  const [accounts, setAccounts] = useState<any[]>([]);
  const [subcategories, setSubcategories] = useState<any[]>([]);
  const [categories, setCategories] = useState<any[]>([]);

  useEffect(() => {
    const fetchData = async () => {
      const headers = { Authorization: `Bearer ${token}` };

      const res1 = await apiFetch("http://localhost:5299/api/Account", {
        headers,
      });
      const res2 = await apiFetch("http://localhost:5299/api/Subcategory", {
        headers,
      });
      const categoryRes = await apiFetch("http://localhost:5299/api/Category");

      const a = await res1.json();
      const s = await res2.json();
      const c = await categoryRes.json();

      setAccounts(a.data.accounts);
      setSubcategories(s.data);
      setCategories(c.data);
    };

    fetchData();
  }, [token]);

  useEffect(() => {
    if (editingTransaction) {
      setAmount(editingTransaction.amount.toString());
      setDescription(editingTransaction.description || "");
      setDate(editingTransaction.date.slice(0, 10));
      setAccountId(
        accounts.find((a) => a.name === editingTransaction.accountName)?.id ||
          null
      );
      const sub = subcategories.find(
        (s) => s.name === editingTransaction.subcategoryName
      );
      setSubcategoryId(sub?.id || null);
      setSelectedCategoryId(sub?.categoryId || null);
      setFilteredSubcategories(
        subcategories.filter((s) => s.categoryId === sub?.categoryId)
      );
    }
  }, [editingTransaction]);

  const handleSubmit = async () => {
    const parsedAmount = parseFloat(amount);
    if (isNaN(parsedAmount)) return;

    if (editingTransaction) {
      await apiFetch(
        `http://localhost:5299/api/Transaction/${editingTransaction.id}`,
        {
          method: "PUT",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            amount: parsedAmount,
            description,
            date,
            accountId,
            subcategoryId,
          }),
        }
      );
      setAmount("0");
      setDescription("");
      setDate(new Date().toISOString().slice(0, 10));
      setAccountId(null);
      setSubcategoryId(null);
      clearEditing();
    } else {
      const res = await apiFetch("http://localhost:5299/api/Transaction", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          amount: parsedAmount,
          description,
          date,
          accountId,
          subcategoryId,
        }),
      });

      if (res.ok) {
        setAmount("0");
        setDescription("");
        setDate(new Date().toISOString().slice(0, 10));
        setAccountId(null);
        setSubcategoryId(null);
      }
    }
    onSaved(); // NOTIFICA il padre che c'è una nuova transazione
  };

  return (
    <div className="mb-6">
      <h2 className="text-lg font-semibold mb-2">Aggiungi nuova spesa</h2>
      <div className="flex gap-2 flex-wrap">
        <input
          type="text"
          placeholder="Importo"
          className="bg-gray-800 text-white border p-2 rounded"
          value={amount}
          onChange={(e) => setAmount(e.target.value)}
        />
        <input
          type="text"
          placeholder="Descrizione"
          className="bg-gray-800 text-white border p-2 rounded"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
        />
        <input
          type="date"
          className="bg-gray-800 text-white border p-2 rounded"
          value={date}
          onChange={(e) => setDate(e.target.value)}
        />
        <select
          className="bg-gray-800 text-white border p-2 rounded"
          value={accountId ?? ""}
          onChange={(e) => setAccountId(parseInt(e.target.value))}
        >
          <option value="">Seleziona conto</option>
          {accounts &&
            accounts.map((acc) => (
              <option key={acc.id} value={acc.id}>
                {acc.name}
              </option>
            ))}
        </select>
        <select
          className="bg-gray-800 text-white border p-2 rounded"
          value={selectedCategoryId ?? ""}
          onChange={(e) => {
            const catId = parseInt(e.target.value);
            setSelectedCategoryId(catId);
            const selectedCat = categories.find((cat) => cat.id === catId);

            const filteredSubcategories = subcategories.filter(
              (sub) => sub.categoryId === selectedCat?.id
            );

            setFilteredSubcategories(filteredSubcategories || []);
            setSubcategoryId(null); // reset subcategory
          }}
        >
          <option value="">Seleziona categoria</option>
          {categories.map((cat) => (
            <option key={cat.id} value={cat.id}>
              {cat.name}
            </option>
          ))}
        </select>
        <select
          className="bg-gray-800 text-white border p-2 rounded"
          value={subcategoryId ?? ""}
          onChange={(e) => setSubcategoryId(parseInt(e.target.value))}
        >
          <option value="">Seleziona sottocategoria</option>
          {filteredSubcategories.map((sub) => (
            <option key={sub.id} value={sub.id}>
              {sub.name}
            </option>
          ))}
        </select>
        <button
          onClick={handleSubmit}
          className="bg-green-600 text-white px-4 py-2 rounded"
        >
          {editingTransaction ? "Aggiorna" : "Salva"}
        </button>
      </div>
    </div>
  );
}
