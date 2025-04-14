"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import router from "next/router";
import { apiFetch } from "@/lib/apiFetch";

interface Account {
  id: number;
  name: string;
  type: string;
  initialBalance: number;
}

export default function AccountsPage() {
  const { token } = useAuth();
  const [accounts, setAccounts] = useState<Account[]>([]);
  const [name, setName] = useState("");
  const [type, setType] = useState("bank");
  const [initialBalance, setInitialBalance] = useState(0);

  const [editingId, setEditingId] = useState<number | null>(null);
  const [editName, setEditName] = useState("");
  const [editType, setEditType] = useState("bank");
  const [editInitialBalance, setEditInitialBalance] = useState(0);

  const fetchAccounts = async () => {
    const res = await apiFetch("http://localhost:5299/api/Account", {
      headers: { Authorization: `Bearer ${token}` },
    });
    const data = await res.json();
    setAccounts(data.data);
  };

  const handleCreate = async () => {
    await apiFetch("http://localhost:5299/api/Account", {
      method: "POST",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({ name, type, initialBalance }),
    });
    setName("");
    setType("bank");
    setInitialBalance(0);
    fetchAccounts();
  };

  const handleDelete = async (id: number) => {
    await apiFetch(`http://localhost:5299/api/Account/${id}`, {
      method: "DELETE",
      headers: { Authorization: `Bearer ${token}` },
    });
    fetchAccounts();
  };

  const handleUpdate = async () => {
    if (editingId === null) return;

    await apiFetch(`http://localhost:5299/api/Account/${editingId}`, {
      method: "PUT",
      headers: {
        Authorization: `Bearer ${token}`,
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        name: editName,
        type: editType,
        initialBalance: editInitialBalance,
      }),
    });

    setEditingId(null);
    fetchAccounts();
  };

  useEffect(() => {
    if (!token) return; // Non chiamare l'API finché non abbiamo il token

    const fetchAccounts = async () => {
      const res = await apiFetch("http://localhost:5299/api/Account", {
        headers: { Authorization: `Bearer ${token}` },
      });
      if (res.status === 401) {
        localStorage.removeItem("token");
        router.push("/login");
      }
      const data = await res.json();
      setAccounts(data.data.accounts);
    };

    fetchAccounts();
  }, [token]);
  return (
    <div className="p-6 bc-ter col-sec ">
      <h1 className="text-2xl font-bold mb-4">Gestione Conti</h1>

      <div className="mb-6">
        <h2 className="text-xl mb-2">Aggiungi nuovo conto</h2>
        <input
          className="border p-2 mr-2"
          placeholder="Nome"
          value={name}
          onChange={(e) => setName(e.target.value)}
        />
        <select
          className="border p-2 mr-2"
          value={type}
          onChange={(e) => setType(e.target.value)}
        >
          <option value="bank">Banca</option>
          <option value="cash">Contanti</option>
          <option value="card">Prepagata</option>
        </select>
        <input
          type="number"
          className="border p-2 mr-2"
          placeholder="Saldo iniziale"
          value={initialBalance}
          onChange={(e) => setInitialBalance(Number(e.target.value))}
        />
        <button
          onClick={handleCreate}
          className="bc-qua text-white px-4 py-2 rounded"
        >
          Aggiungi
        </button>
      </div>

      <h2 className="text-xl mb-2">Tutti i conti</h2>
      <table className="w-full border">
        <thead>
          <tr className="bg-gray-200 text-left">
            <th className="p-2">Nome</th>
            <th className="p-2">Tipo</th>
            <th className="p-2">Saldo</th>
            <th className="p-2">Azioni</th>
          </tr>
        </thead>
        <tbody>
          {accounts &&
            accounts.map((account) => (
              <tr key={account.id} className="border-t">
                <td className="p-2">{account.name}</td>
                <td className="p-2">{account.type}</td>
                <td className="p-2">€ {account.initialBalance.toFixed(2)}</td>
                <td className="p-2">
                  <button
                    onClick={() => {
                      setEditingId(account.id);
                      setEditName(account.name);
                      setEditType(account.type);
                      setEditInitialBalance(account.initialBalance);
                    }}
                    className="bg-yellow-500 text-white px-3 py-1 rounded mr-2"
                  >
                    Modifica
                  </button>
                  <button
                    onClick={() => handleDelete(account.id)}
                    className="bg-red-500 text-white px-3 py-1 rounded"
                  >
                    Elimina
                  </button>
                </td>
              </tr>
            ))}
        </tbody>
      </table>
      {editingId && (
        <div className="mt-6 border-t pt-6">
          <h2 className="text-xl mb-2">Modifica conto</h2>
          <input
            className="border p-2 mr-2"
            placeholder="Nome"
            value={editName}
            onChange={(e) => setEditName(e.target.value)}
          />
          <select
            className="border p-2 mr-2"
            value={editType}
            onChange={(e) => setEditType(e.target.value)}
          >
            <option value="bank">Banca</option>
            <option value="cash">Contanti</option>
            <option value="card">Prepagata</option>
          </select>
          <input
            type="number"
            className="border p-2 mr-2"
            placeholder="Saldo iniziale"
            value={editInitialBalance}
            onChange={(e) => setEditInitialBalance(Number(e.target.value))}
          />
          <button
            onClick={handleUpdate}
            className="bg-green-600 text-white px-4 py-2 rounded mr-2"
          >
            Salva modifiche
          </button>
          <button
            onClick={() => setEditingId(null)}
            className="bg-gray-400 text-white px-4 py-2 rounded"
          >
            Annulla
          </button>
        </div>
      )}
    </div>
  );
}
