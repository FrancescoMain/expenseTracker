"use client";

import { useAuth } from "@/context/AuthContext";
import { useState } from "react";
import { format } from "date-fns";

export default function ReportsPage() {
  const { token } = useAuth();
  const [startDate, setStartDate] = useState(() =>
    new Date(new Date().getFullYear(), new Date().getMonth(), 1)
      .toISOString()
      .slice(0, 10)
  );
  const [endDate, setEndDate] = useState(() =>
    new Date().toISOString().slice(0, 10)
  );

  const [summary, setSummary] = useState<any | null>(null);

  const handleDownload = async () => {
    const res = await fetch(
      `http://localhost:5299/api/Report/export?start=${startDate}&end=${endDate}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    if (!res.ok) {
      alert("Errore durante il download del report.");
      return;
    }

    const blob = await res.blob();
    const url = window.URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = `report_${startDate}_to_${endDate}.xlsx`;
    a.click();
    window.URL.revokeObjectURL(url);
  };

  const handleLoadSummary = async () => {
    const res = await fetch(
      `http://localhost:5299/api/Report/summary?start=${startDate}&end=${endDate}`,
      {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    const data = await res.json();
    setSummary(data.data);
  };

  return (
    <div className="p-6 bc-ter">
      <h1 className="text-2xl font-bold mb-6">📄 Report Mensile</h1>

      <div className="flex gap-4 mb-4">
        <div>
          <label className="block mb-1 text-sm">Data Inizio</label>
          <input
            type="date"
            className="col-sec text-white border p-2 rounded"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
        </div>

        <div>
          <label className="block mb-1 text-sm">Data Fine</label>
          <input
            type="date"
            className="col-sec text-white border p-2 rounded"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
        </div>

        <button
          onClick={handleDownload}
          className="bg-green-600 text-white px-4 py-2 rounded mt-6"
        >
          Scarica Excel
        </button>

        <button
          onClick={handleLoadSummary}
          className="bg-blue-600 text-white px-4 py-2 rounded mt-6"
        >
          Carica Riepilogo
        </button>
      </div>

      {summary && (
        <div className="mt-6">
          <h2 className="text-xl font-semibold mb-2">📊 Riepilogo</h2>
          <ul className="list-disc pl-5 text-sm">
            <li>
              <strong>Spese Totali:</strong>{" "}
              <span className="text-red-500">
                € {summary.totalExpenses.toFixed(2)}
              </span>
            </li>
            <li>
              <strong>Guadagni Totali:</strong>{" "}
              <span className="text-green-500">
                € {summary.totalIncome.toFixed(2)}
              </span>
            </li>
            <li>
              <strong>Bilancio Netto:</strong>{" "}
              <span
                className={
                  summary.netBalance < 0
                    ? "text-red-600"
                    : "text-green-600 font-bold"
                }
              >
                € {summary.netBalance.toFixed(2)}
              </span>
            </li>
          </ul>

          <h3 className="text-lg mt-4 font-semibold">Totale per Categoria:</h3>
          <ul className="pl-5 mt-1 text-sm">
            {Object.entries(summary.byCategory).map(([cat, val]: any) => (
              <li key={cat}>
                <span className="font-medium">{cat}:</span> € {val.toFixed(2)}
              </li>
            ))}
          </ul>

          <div className="mt-4 text-sm">
            <strong>Saldo Totale Attuale:</strong>{" "}
            <span className="font-bold text-blue-400">
              € {summary.totalBalance.toFixed(2)}
            </span>
          </div>
        </div>
      )}
    </div>
  );
}
