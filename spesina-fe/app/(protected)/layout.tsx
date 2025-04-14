// app/(protected)/layout.tsx
"use client";

import { useAuth } from "@/context/AuthContext";
import { useRouter } from "next/navigation";
import React from "react";
import Sidebar from "@/components/Sidebar";

export default function ProtectedLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const { user, isLoading } = useAuth();
  const router = useRouter();

  React.useEffect(() => {
    if (!isLoading && !user) {
      router.push("/login");
    }
  }, [isLoading, user]);

  if (isLoading || !user) return <div>Caricamento...</div>;

  return (
    <div className="flex min-h-screen bc-prim">
      <Sidebar />
      <div className="flex-1 p-6">{children}</div>
    </div>
  );
}
